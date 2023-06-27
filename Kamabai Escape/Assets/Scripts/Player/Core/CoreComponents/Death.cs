using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : CoreComponent
{
    //[SerializeField] private GameObject[] deathParticles;

    public void Die()
    {
        //Place Particle effects here

        core.transform.parent.gameObject.SetActive(false);
    }
}
