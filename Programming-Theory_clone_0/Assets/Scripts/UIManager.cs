using System.Collections;
using UnityEngine;
using TMPro;
using PurrNet;
using System.Collections.Generic;

public class UIManager : NetworkIdentity
{
    public float timer = 3f;
    private bool _canCount = false;
    private TMP_Text _startCounter;
    private GameObject _controls;

    // Server-side ready tracking
    private readonly HashSet<PlayerID> _readyPlayers = new();

    public static void SetCursorVisibility(bool visibility)
    {
        Cursor.lockState = visibility ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visibility;
    }

    protected override void OnSpawned(bool asServer)
    {
        SetCursorVisibility(false);
        _startCounter = GameObject.Find("Start Counter").GetComponent<TMP_Text>();
        _startCounter.enabled = true;
        _startCounter.gameObject.SetActive(false);

        StartCoroutine(ShowControlsThenReportReady());
    }

    private IEnumerator ShowControlsThenReportReady()
    {
        // Show the correct control formation for this player's car
        if (GameManager.Instance.choosenCarType == GameManager.Cars.Convertible ||
            GameManager.Instance.choosenCarType == GameManager.Cars.Pickup)
        {
            _controls = GameObject.Find("Formation 1");
            GameObject.Find("Formation 2").SetActive(false);
        }
        else if (GameManager.Instance.choosenCarType == GameManager.Cars.Jumpy ||
            GameManager.Instance.choosenCarType == GameManager.Cars.SharkTruck)
        {
            _controls = GameObject.Find("Formation 2");
            GameObject.Find("Formation 1").SetActive(false);
        }

        _controls.SetActive(true);

        // Wait for controls screen, then tell server this client is ready
        yield return new WaitForSeconds(7f);

        if (isServer)
            PlayerReadyOnServer(localPlayer.Value);
        else
            ReportReadyServerRpc();
    }

    [ServerRpc(requireOwnership: false)]
    private void ReportReadyServerRpc(RPCInfo info = default)
    {
        PlayerReadyOnServer(info.sender);
    }

    private void PlayerReadyOnServer(PlayerID player)
    {
        _readyPlayers.Add(player);

        LobbyManager lobbyManager = FindAnyObjectByType<LobbyManager>();
        int expectedPlayers = lobbyManager != null ? lobbyManager.lobbySettings.playerCount : 1;

        Debug.Log($"[UIManager] {_readyPlayers.Count}/{expectedPlayers} players ready.");

        if (_readyPlayers.Count >= expectedPlayers)
            StartCountdownObserversRpc();
    }

    [ObserversRpc]
    private void StartCountdownObserversRpc()
    {
        _startCounter.gameObject.SetActive(true);
        _canCount = true;
    }

    private void Update()
    {
        if (_canCount)
        {
            if (timer >= 0f)
                timer -= Time.smoothDeltaTime;

            _startCounter.text = Mathf.RoundToInt(timer % 60).ToString();

            if (timer <= .5f)
            {
                GameManager.IsGameStarted = true;
                _startCounter.text = "GO!";
                StartCoroutine(HideTimer());
            }
        }
    }

    private IEnumerator HideTimer()
    {
        yield return new WaitForSeconds(2f);
        _startCounter.gameObject.SetActive(false);
        _canCount = false;
    }
}
