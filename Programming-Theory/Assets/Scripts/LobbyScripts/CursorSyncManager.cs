using PurrNet;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorSyncManager : NetworkBehaviour
{
    [Header("References")]
    public RectTransform cursorContainer;
    public Sprite cursorSprite;
    public Canvas rootCanvas;

    [Header("Car Selection")]
    public MenuController menuController;

    [Header("Settings")]
    public float remoteCursorAlpha = 0.4f;
    public Vector2 cursorSize = new Vector2(24f, 32f);
    public Color takenOverlayColor = new Color(0f, 0f, 0f, 0.6f);
    public Color blockedHoverColor = new Color(0.8f, 0f, 0f, 0.4f); // Red tint on hover

    private readonly Dictionary<PlayerID, RectTransform> _remoteCursors = new();
    private readonly Dictionary<GameManager.Cars, PlayerID> _claimedCars = new();

    private float _sendInterval = 0.05f; // 20 times/sec
    private float _sendTimer;

    protected override void OnSpawned(bool asServer)
    {
        if (rootCanvas == null)
            rootCanvas = GetComponentInParent<Canvas>() ?? FindAnyObjectByType<Canvas>();
        
        if (cursorContainer == null) cursorContainer = rootCanvas.transform.Find("ChooseCarScreen").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActiveAndEnabled) return;

        _sendTimer -= Time.deltaTime;
        if (_sendTimer <= 0f)
        {
            _sendTimer = _sendInterval;
            BroadcastCursorPosition();
        }
    }

    private void BroadcastCursorPosition()
    {
        if (!localPlayer.HasValue) return;

        Vector2 screenPos = Input.mousePosition;

        if (isServer)
            OnCursorMovedObserversRpc(localPlayer.Value, screenPos);
        else
            SendCursorPositionServerRpc(screenPos);
    }

    [ServerRpc(requireOwnership: false)]
    private void SendCursorPositionServerRpc(Vector2 screenPos, RPCInfo info = default)
    {
        OnCursorMovedObserversRpc(info.sender, screenPos);
    }

    [ObserversRpc]
    private void OnCursorMovedObserversRpc(PlayerID owner, Vector2 screenPos)
    {
        // Don't render a remote cursor for ourselves
        if (owner == localPlayer) return;

        if (!_remoteCursors.TryGetValue(owner, out RectTransform cursorRect))
        {
            cursorRect = CreateRemoteCursor(owner);
            _remoteCursors[owner] = cursorRect;
        }

        // Convert screen position to local position within the canvas
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.GetComponent<RectTransform>(),
            screenPos,
            rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera,
            out Vector2 localPoint))
        {
            cursorRect.localPosition = localPoint;
        }
    }

    private RectTransform CreateRemoteCursor(PlayerID owner)
    {
        GameObject cursorObj = new GameObject($"RemoteCursor_{owner}");
        cursorObj.transform.SetParent(cursorContainer, false);

        RectTransform rect = cursorObj.AddComponent<RectTransform>();
        rect.sizeDelta = cursorSize;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.pivot = new Vector2(0f, 1f); // Top-left like a real cursor

        Image img = cursorObj.AddComponent<Image>();
        img.sprite = cursorSprite;
        img.raycastTarget = false; // Don't block UI clicks

        // Semi-transparent to indicate it's not yours
        Color c = img.color;
        c.a = remoteCursorAlpha;
        img.color = c;

        return rect;
    }

    // Call this when a player leaves or the screen closes
    public void RemoveRemoteCursor(PlayerID owner)
    {
        if (_remoteCursors.TryGetValue(owner, out RectTransform rect))
        {
            if (rect != null) Destroy(rect.gameObject);
            _remoteCursors.Remove(owner);
        }
    }

    public void ClearAllRemoteCursors()
    {
        foreach (var kvp in _remoteCursors)
            if (kvp.Value != null) Destroy(kvp.Value.gameObject);

        _remoteCursors.Clear();
    }

    public bool IsCarTaken(GameManager.Cars car)
    {
        if (!localPlayer.HasValue) return false;
        return _claimedCars.TryGetValue(car, out PlayerID owner) && owner != localPlayer.Value;
    }

    public void LocalPlayerSelectedCar(GameManager.Cars car)
    {
        if (!localPlayer.HasValue) return;

        // Block selection of a taken car
        if (IsCarTaken(car)) return;

        if (isServer)
            HandleCarSelectionOnServer(localPlayer.Value, car);
        else
            RequestCarSelectionServerRpc(car);
    }

    [ServerRpc(requireOwnership: false)]
    private void RequestCarSelectionServerRpc(GameManager.Cars car, RPCInfo info = default)
    {
        HandleCarSelectionOnServer(info.sender, car);
    }

    private void HandleCarSelectionOnServer(PlayerID sender, GameManager.Cars car)
    {
        if (_claimedCars.TryGetValue(car, out PlayerID owner) && owner != sender)
            return;

        GameManager.Cars? previousCar = null;
        foreach (var kvp in _claimedCars)
        {
            if (kvp.Value == sender) { previousCar = kvp.Key; break; }
        }
        if (previousCar.HasValue)
            _claimedCars.Remove(previousCar.Value);

        _claimedCars[car] = sender;

        // Check if all players have selected — compare against LobbyManager's player count
        LobbyManager lobbyManager = FindAnyObjectByType<LobbyManager>();
        int totalPlayers = lobbyManager != null ? lobbyManager.lobbySettings.playerCount : 1;
        bool allReady = _claimedCars.Count >= totalPlayers;

        BroadcastCarClaimsObserversRpc(sender, car, previousCar ?? (GameManager.Cars)(-1), previousCar.HasValue, allReady);
    }

    [ObserversRpc]
    private void BroadcastCarClaimsObserversRpc(PlayerID claimer, GameManager.Cars newCar, GameManager.Cars releasedCar, bool hadPrevious, bool allReady)
    {
        if (hadPrevious)
            SetCarOverlay(releasedCar, OverlayState.None);

        bool isLocalPlayer = claimer == localPlayer;
        SetCarOverlay(newCar, isLocalPlayer ? OverlayState.OwnSelection : OverlayState.Taken);

        // Update Start button — only host sees it but lock it universally until all ready
        if (menuController != null)
            menuController.SetStartButtonReady(allReady);
    }

    private enum OverlayState { None, Taken, OwnSelection, BlockedHover }

    private void SetCarOverlay(GameManager.Cars car, OverlayState state)
    {
        if (menuController == null) return;

        GameObject selectionObj = menuController.GetSelectionObject(car);
        if (selectionObj == null) return;

        Transform overlayTransform = selectionObj.transform.Find("GreyOverlay");

        if (state == OverlayState.None)
        {
            if (overlayTransform != null)
                overlayTransform.gameObject.SetActive(false);
            return;
        }

        // Create overlay if it doesn't exist
        Image img;
        if (overlayTransform == null)
        {
            GameObject overlay = new GameObject("GreyOverlay");
            overlay.transform.SetParent(selectionObj.transform, false);

            RectTransform rect = overlay.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            img = overlay.AddComponent<Image>();
            img.raycastTarget = false;
            overlay.transform.SetAsLastSibling();
        }
        else
        {
            overlayTransform.gameObject.SetActive(true);
            img = overlayTransform.GetComponent<Image>();
        }

        img.color = state switch
        {
            OverlayState.Taken => takenOverlayColor,
            OverlayState.BlockedHover => blockedHoverColor,
            OverlayState.OwnSelection => new Color(0f, 0.6f, 0f, 0.35f), // Green tint = yours
            _ => Color.clear
        };
    }

    // Called by MenuController on pointer enter/exit
    public void OnCarHoverEnter(GameManager.Cars car)
    {
        if (IsCarTaken(car))
            SetCarOverlay(car, OverlayState.BlockedHover);
    }

    public void OnCarHoverExit(GameManager.Cars car)
    {
        if (IsCarTaken(car))
            SetCarOverlay(car, OverlayState.Taken);
    }
}

