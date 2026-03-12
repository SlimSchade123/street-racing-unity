using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BoostPad : MonoBehaviour
{
    [Tooltip("A speed boost modifier greater than 1 will increase the car's speed. A speed boost modifier less than 1 will decrease the car's speed. A speed boost modifier of 1 will do nothing.")]
    [SerializeField] protected float speedBoostModifier = 1;

    //Starts speed boost when car enters
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<CarController>() != null)
        {
            other.gameObject.GetComponentInParent<CarController>().StartSpeedBoost(speedBoostModifier);
        }
    }

    //Ends speed boost when car exits
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInParent<CarController>() != null)
        {
            other.gameObject.GetComponentInParent<CarController>().EndSpeedBoost();
        }
    }
}
