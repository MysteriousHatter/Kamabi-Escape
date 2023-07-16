using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{

    void Start()
    {

    }
    // Start is called before the first frame update


    private void OnCollisionEnter(Collision other)
    {
        // the following two lines calculate if the other object is in front or behind the player
        // ensures that the other object is a hazard
        var playerTag = other.gameObject.tag;
        if (playerTag == "Player" || playerTag == "Drill")
        { 
            Debug.Log("Hit Hazard");
        }
    }
}
