using System;
using System.Collections;
using UnityEngine;

<<<<<<< Updated upstream
[RequireComponent (typeof(BoxCollider))]
=======
[RequireComponent(typeof(BoxCollider))]
>>>>>>> Stashed changes
public class ItemPickup : MonoBehaviour
{
    [Tooltip("Respawns pickup after specified amount of time. (To not despawn: set to 0), (To not respawn, set to a negative number)")]
    [SerializeField] float respawnTimer = -1f;

    private void OnTriggerEnter(Collider other)
    {
<<<<<<< Updated upstream
        if(other.gameObject.GetComponentInParent<CarController>() != null)
=======
        if (other.gameObject.GetComponentInParent<CarController>() != null)
>>>>>>> Stashed changes
        {
            ExecuteFunctionality(other.gameObject.GetComponentInParent<CarController>());

            //"Despawn" object when picked up (unless told not to do so)
<<<<<<< Updated upstream
            if(respawnTimer != 0)
=======
            if (respawnTimer != 0)
>>>>>>> Stashed changes
            {
                this.gameObject.GetComponent<Collider>().enabled = false;
                this.gameObject.GetComponent<Renderer>().enabled = false;
            }

            //Respawn object after designated amount of time (if respawnTimer is greater than 0)
<<<<<<< Updated upstream
            if(respawnTimer > 0)
=======
            if (respawnTimer > 0)
>>>>>>> Stashed changes
            {
                StartCoroutine(Respawn(respawnTimer));
            }
        }
    }

    //Respawn pickup after set amount of time
    private IEnumerator Respawn(float timer)
    {
        yield return new WaitForSeconds(timer);

        this.gameObject.GetComponent<Collider>().enabled = true;
        this.gameObject.GetComponent<Renderer>().enabled = true;
    }

    //Allows child classes to execute any given functionality they need to when picked up
    protected virtual void ExecuteFunctionality(CarController carController)
    {
        //Here for inheritence
    }
}
