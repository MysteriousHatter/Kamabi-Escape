using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblingPlatform : MonoBehaviour
{
    public float vanishThreshold = 3f; // Time threshold for platform vanishing
    Animator myAnim => GetComponent<Animator>();
    private bool isPlayerOnPlatform = false;

    [SerializeField] bool canReset;
    [SerializeField] float resetTime;


    private void Start()
    {
        myAnim.SetFloat("DisappearTime", 1 / vanishThreshold);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //isPlayerOnPlatform = true;
            myAnim.SetBool("Trigger", true);
            //dStartCoroutine(CheckVanishTimer());
        }
    }

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        isPlayerOnPlatform = false;
    //    }
    //}

    //private IEnumerator CheckVanishTimer()
    //{
    //    yield return new WaitForSeconds(1/vanishThreshold);

    //    if (isPlayerOnPlatform)
    //    {
    //        // Vanish the platform
    //        gameObject.SetActive(false);

    //        // Perform additional actions/effects here
    //    }
    //}

    public void TriggerReset()
    {
        if(canReset)
        {
            StartCoroutine(Reset());
        }
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(resetTime);
        myAnim.SetBool("Trigger", false);
    }
}
