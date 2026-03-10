using PurrNet;
using PurrNet.Modules;
using UnityEngine;

public class CarSpawner : NetworkIdentity
{
    [SerializeField] CarController Convertible;
    [SerializeField] CarController Pickup;
    [SerializeField] CarController Jumpy;
    [SerializeField] CarController Shark;

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
        GameObject carObject = carChoice switch
        {
            GameManager.Cars.Convertible => Instantiate(Convertible.gameObject, transform.position, transform.rotation),
            GameManager.Cars.Pickup => Instantiate(Pickup.gameObject, transform.position, transform.rotation),
            GameManager.Cars.Jumpy => Instantiate(Jumpy.gameObject, transform.position, transform.rotation),
            GameManager.Cars.SharkTruck => Instantiate(Shark.gameObject, transform.position, transform.rotation),
            _ => null
        };

        if (carObject != null)
        {
            var identity = carObject.GetComponent<NetworkIdentity>();
            identity.GiveOwnership(player);
        }
    }
}
