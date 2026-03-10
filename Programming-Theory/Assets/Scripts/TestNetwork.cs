using PurrNet;
using PurrNet.Modules;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : NetworkIdentity
{
    [SerializeField] CarController Convertible;
    [SerializeField] CarController Pickup;
    [SerializeField] CarController Jumpy;
    [SerializeField] CarController Shark;

    [SerializeField] private Vector3 spawnOffset = new Vector3(5f, -2f, 0f);

    private readonly Dictionary<PlayerID, int> _playerIndices = new();
    private int _nextIndex = 0;

    protected override void OnSpawned(bool asServer)
    {
        if (!asServer) return;

        if (networkManager.TryGetModule<PlayersManager>(true, out var playersManager))
        {
            playersManager.onPlayerJoined += OnPlayerJoined;
        }
    }

    private void OnPlayerJoined(PlayerID player, bool isReconnect, bool asServer)
    {
        // Server spawns server's car, clients need to request their own
        if (player == localPlayer)
        {
            SpawnCarForPlayer(player, GameManager.Instance.choosenCarType);
        }
    }

    // Client calls this RPC to request their car
    [ServerRpc(requireOwnership: false)]
    public void ServerRequestSpawnCar(GameManager.Cars carChoice, RPCInfo info = default)
    {
        SpawnCarForPlayer(info.sender, carChoice);
    }

    private void SpawnCarForPlayer(PlayerID player, GameManager.Cars carChoice)
    {
        if (!_playerIndices.ContainsKey(player))
            _playerIndices[player] = _nextIndex++;

        Vector3 spawnPos = transform.position + (spawnOffset * _playerIndices[player]);
        Quaternion spawnRot = transform.rotation;

        Debug.Log($"Spawning for {player} at index {_playerIndices[player]}, pos {spawnPos}");
        GameObject carObject = carChoice switch
        {
            GameManager.Cars.Convertible => Instantiate(Convertible.gameObject, spawnPos, spawnRot),
            GameManager.Cars.Pickup => Instantiate(Pickup.gameObject, spawnPos, spawnRot),
            GameManager.Cars.Jumpy => Instantiate(Jumpy.gameObject, spawnPos, spawnRot),
            GameManager.Cars.SharkTruck => Instantiate(Shark.gameObject, spawnPos, spawnRot),
            _ => null
        };

        if (carObject != null)
        {
            var identity = carObject.GetComponent<NetworkIdentity>();
            identity.GiveOwnership(player);
            networkManager.Spawn(carObject);
        }
    }
}
