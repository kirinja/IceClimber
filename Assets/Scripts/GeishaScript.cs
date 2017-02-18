using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeishaScript : MonoBehaviour
{
    public float force;
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           other.GetComponent<Rigidbody>().AddForce(Vector3.up*force);
        }
    }
	
}
