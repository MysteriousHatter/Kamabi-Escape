using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblingPlatform : MonoBehaviour
{
    public float vanishThreshold = 3f; // Time threshold for platform vanishing

    private bool isPlayerOnPlatform = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = true;
            StartCoroutine(CheckVanishTimer());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = false;
        }
    }

    private IEnumerator CheckVanishTimer()
    {
        yield return new WaitForSeconds(vanishThreshold);

        if (isPlayerOnPlatform)
        {
            // Vanish the platform
            gameObject.SetActive(false);

            // Perform additional actions/effects here
        }
    }
}
