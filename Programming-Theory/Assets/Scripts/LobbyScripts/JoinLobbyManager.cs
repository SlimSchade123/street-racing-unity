using PurrNet;
using System.Collections.Generic;
using UnityEngine;

public class JoinLobbyManager : NetworkIdentity
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject JoinLobbyPrefab;
    public List<LobbyManager> Lobbies;

    public GameObject ScrollViewContent;

    public static JoinLobbyManager instance;
    private NetworkManager networkManager;
    protected override void OnSpawned(bool asServer)
    {
        instance = this;
        networkManager = FindAnyObjectByType<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void SpawnLobbyOnServer()
    {
        GameObject lobby = Instantiate(JoinLobbyPrefab);
        lobby.transform.SetParent(ScrollViewContent.transform, false);

        networkManager.Spawn(lobby);

        var lobbyManager = lobby.GetComponent<LobbyManager>();
        if (lobbyManager != null)
        {
            Lobbies.Add(lobbyManager);
        }

        // Notify all clients to update their UI
        var networkIdentity = lobby.GetComponent<NetworkIdentity>();
        OnLobbyCreatedObserversRpc(networkIdentity.id);
    }

    [ServerRpc(requireOwnership: false)]
    private void RequestCreatLobbyServerRpc()
    {
        SpawnLobbyOnServer();
    }

    [ObserversRpc]
    private void OnLobbyCreatedObserversRpc(NetworkID? lobbyNetworkID)
    {
        // Find the spawned lobby by its network ID and re-parent it
        var allLobbies = FindObjectsByType<LobbyManager>(FindObjectsSortMode.None);
        foreach (var lobbyManager in allLobbies)
        {
            // If not already parented to ScrollViewContent, re-parent it
            if (lobbyManager.transform.parent != ScrollViewContent.transform)
            {
                lobbyManager.transform.SetParent(ScrollViewContent.transform, false);
            }

            if (!Lobbies.Contains(lobbyManager))
            {
                Lobbies.Add(lobbyManager);
            }
        }
        RefreshLobbyList();
        Debug.Log($"Lobby {lobbyNetworkID} created and synced to clients");
    }

    public void CreateLobby()
    {
        if (isServer)
        {
            SpawnLobbyOnServer();
        }
        else
        {
            RequestCreatLobbyServerRpc();
        }
    }

    private void RefreshLobbyList()
    {
        foreach (Transform child in ScrollViewContent.transform)
        {
            var lobbyManager = child.GetComponent<LobbyManager>();
            if (lobbyManager != null && !Lobbies.Contains(lobbyManager))
            {
                Lobbies.Add(lobbyManager);
            }
        }
    }
}
