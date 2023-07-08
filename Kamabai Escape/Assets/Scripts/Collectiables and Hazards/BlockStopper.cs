using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockStopper : MonoBehaviour
{
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Stop the object's movement when it collides with another object
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
