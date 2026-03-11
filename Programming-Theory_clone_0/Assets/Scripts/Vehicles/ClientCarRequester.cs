using PurrNet;
using UnityEngine;

public class ClientCarRequester : NetworkBehaviour
{
    private static bool _hasRequestedCar = false;

    protected override void OnSpawned(bool asServer)
    {
        // Only clients (not host) need to request
        if (asServer || _hasRequestedCar) return;

        _hasRequestedCar = true;

        // Find the spawner and request our car
        var spawner = FindFirstObjectByType<CarSpawner>();
        if (spawner != null)
        {
            spawner.ServerRequestSpawnCar(GameManager.Instance.choosenCarType);
        }
    }

    private void OnDestroy()
    {
        _hasRequestedCar = false;
    }
}