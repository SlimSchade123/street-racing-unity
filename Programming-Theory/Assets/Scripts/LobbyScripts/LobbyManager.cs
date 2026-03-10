using PurrNet;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : NetworkIdentity
{
    public LobbySettings lobbySettings;
    public LobbyPlayer HostPlayer;
    public List<LobbyPlayer> ClientPlayers = new();
    public bool IsLobbyFull => lobbySettings.playerCount >= lobbySettings.maxPlayers;
    public NetworkID? LobbyID { get; private set; }
    public Button JoinButton;
    public GameObject PlayerPanelPrefab;
    public GameObject ScrollViewContent;
    public GameObject LobbyPanel;
    public TMP_InputField PlayerNameField;
    private NetworkManager _networkManager;

    override protected void OnSpawned(bool asServer)
    {
        _networkManager = FindAnyObjectByType<NetworkManager>();
        LobbyID = this.id;
        lobbySettings = new LobbySettings()
        {
            maxPlayers = 4,
            playerCount = 0,
            speedMultiplier = 1
        };

        JoinButton = GetComponentInChildren<Button>();
        JoinButton.onClick.RemoveAllListeners();
        JoinButton.onClick.AddListener(() => JoinLobby());
        LobbyPanel = transform.parent.parent.parent.parent.parent.Find("Lobby").gameObject;
        ScrollViewContent = LobbyPanel.transform.Find("Scroll View/Viewport/Content").gameObject;
        
        PlayerNameField = JoinLobbyManager.instance.transform.Find("TopPanel/NameField").GetComponentInChildren<TMP_InputField>();
    }

    void Update()
    {
        if (IsLobbyFull && JoinButton != null)
        {
            JoinButton.interactable = false;
        }
    }

    public void JoinLobby()
    {
        if (IsLobbyFull) return;

        string playerName = PlayerNameField != null ? PlayerNameField.text : "Player";
        if (string.IsNullOrEmpty(playerName)) playerName = "Player";

        if (isServer)
        {
            if (localPlayer.HasValue)
            {
                SpawnPlayerOnServer(localPlayer.Value, playerName);
            }
        }
        else
        {
            RequestJoinLobbyServerRpc(playerName);
        }
    }

    [ServerRpc(requireOwnership: false)]
    private void RequestJoinLobbyServerRpc(string playerName, RPCInfo info = default)
    {
        SpawnPlayerOnServer(info.sender, playerName);
    }

    private void SpawnPlayerOnServer(PlayerID sender, string playerName)
    {
        if (IsLobbyFull) return;

        GameObject playerObj = Instantiate(PlayerPanelPrefab);
        _networkManager.Spawn(playerObj);

        LobbyPlayer player = playerObj.GetComponent<LobbyPlayer>();
        player.playerName = playerName;

        if (lobbySettings.playerCount == 0)
        {
            HostPlayer = player;
        }
        else
        {
            ClientPlayers.Add(player);
        }

        lobbySettings.playerCount++;

        if (ScrollViewContent == null) Debug.LogError("OH NOOOOO");
        playerObj.transform.SetParent(ScrollViewContent.transform, false);

        Debug.Log($"Player {player.playerName} joined lobby {LobbyID}. Current player count: {lobbySettings.playerCount}");

        OnPlayerJoinedObserversRpc(sender, playerName);
    }

    [ObserversRpc]
    private void OnPlayerJoinedObserversRpc(PlayerID joiningPlayer, string playerName)
    {
        bool isJoiningPlayer = joiningPlayer == localPlayer;

        if (isJoiningPlayer)
        {
            if (LobbyPanel != null)
            {
                LobbyPanel.SetActive(true);
            }
            gameObject.SetActive(false);
        }

        var allPlayers = FindObjectsByType<LobbyPlayer>(FindObjectsSortMode.None);
        foreach (var player in allPlayers)
        {
            // Only set name for the newly joined player (who has default name)
            if (player.playerName == "TestPlayer" || string.IsNullOrEmpty(player.playerName))
            {
                player.playerName = playerName;
                player.GetComponentInChildren<TMP_Text>().text = playerName;
            }

            if (player.transform.parent != ScrollViewContent.transform)
            {
                player.transform.SetParent(ScrollViewContent.transform, false);
            }

            if (player != HostPlayer && !ClientPlayers.Contains(player))
            {
                ClientPlayers.Add(player);
            }
        }

        Debug.Log($"Player {playerName} joined lobby {LobbyID}. Total players in content: {ScrollViewContent.transform.childCount}");
    }

    public void PopulateLobby()
    {
        
    }
}
