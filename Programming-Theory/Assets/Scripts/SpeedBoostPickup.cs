using UnityEngine;

public class SpeedBoostPickup : ItemPickup
{
    [SerializeField] protected float speedBoost = 1;
    [SerializeField] protected float duration = 1;

    protected override void ExecuteFunctionality(CarController carController)
    {
        carController.BoostSpeed(speedBoost, duration);
    }
}
