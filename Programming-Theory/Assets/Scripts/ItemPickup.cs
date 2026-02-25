using System;
using UnityEngine;

[RequireComponent (typeof(SphereCollider))]
public class ItemPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        print("Trigger Enter");
        if(other.gameObject.GetComponentInParent<CarController>() != null)
        {
            ExecuteFunctionality(other.gameObject.GetComponentInParent<CarController>());
        }
    }


    protected virtual void ExecuteFunctionality(CarController carController)
    {
        //Here for inheritence
    }
}
