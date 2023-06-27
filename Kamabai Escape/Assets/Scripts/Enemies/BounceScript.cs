using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceScript : MonoBehaviour
{
    public float bounceForce = 10f; // Adjust the force to determine the height of the bounce

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Bounce Rigidbody");
            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                Vector3 bounceDirection = transform.up; // Adjust the direction of the bounce if necessary
                playerRigidbody.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
            }
        }
    }
}
