using PurrNet;
using UnityEngine;

public class TestNetwork : NetworkIdentity
{
    // Network identities need to be ready on the server, so instead of using "Start", we use "OnSpawned"

    [SerializeField] NetworkIdentity networkIdentity;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        Instantiate(networkIdentity);
    }
}
