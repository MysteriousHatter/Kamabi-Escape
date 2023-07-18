using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    private ParticleSystem part;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Hit Hazard");
            other.gameObject.GetComponent<Player>().PlayerDeath();
        }
    }
}
