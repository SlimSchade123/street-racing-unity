using UnityEngine;

public class SpeedBoostPickup : ItemPickup
{
    [SerializeField] protected float speedBoost = 1;
    [SerializeField] protected float duration = 1;

    //Boosts the speed of the player that picked this SpeedBoostPickup up by a specified amount for a specified duration of time.
    protected override void ExecuteFunctionality(CarController carController)
    {
        carController.BoostSpeedForTime(speedBoost, duration);
    }
}
