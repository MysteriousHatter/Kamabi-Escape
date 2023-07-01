using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spike : MonoBehaviour
{
    public float popInterval = 2f; // Time interval between spike pop-ins and pop-outs
    public float popDuration = 0.5f; // Duration of the spike pop-in and pop-out animations
    public AnimationCurve popCurve; // Animation curve for the spike pop-in and pop-out animations
    public Vector3 shootDirection = Vector3.up; // Direction in which the spikes shoot out

    private bool isPoppedOut = false;
    private Vector3 poppedOutPosition;
    private Vector3 poppedInPosition;

    private void Start()
    {
        poppedOutPosition = transform.position;
        poppedInPosition = transform.position - shootDirection * transform.localScale.y;

        InvokeRepeating("ToggleSpikeState", popInterval, popInterval);
    }

    private void ToggleSpikeState()
    {
        if (isPoppedOut)
        {
            StartCoroutine(MoveSpike(poppedInPosition, poppedOutPosition));
        }
        else
        {
            StartCoroutine(MoveSpike(poppedOutPosition, poppedInPosition));
        }

        isPoppedOut = !isPoppedOut;
    }

    private IEnumerator MoveSpike(Vector3 startPosition, Vector3 targetPosition)
    {
        float elapsedTime = 0f;

        while (elapsedTime < popDuration)
        {
            float t = elapsedTime / popDuration;
            float curveValue = popCurve.Evaluate(t);
            transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }

    private void OnCollisionEnter(Collision other)
    {
        // Check if the other object is a hazard
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Drill"))
        {
            Debug.Log("Hit Hazard");
        }
    }
}
