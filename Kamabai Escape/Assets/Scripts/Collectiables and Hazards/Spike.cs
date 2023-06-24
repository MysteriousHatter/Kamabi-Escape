using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class Spike : MonoBehaviour
{
    public float popInterval = 2f; // Time interval between spike pop-ins and pop-outs
    public float popDuration = 0.5f; // Duration of the spike pop-in and pop-out animations
    public AnimationCurve popCurve; // Animation curve for the spike pop-in and pop-out animations

    private bool isPoppedOut = false;
    private Vector3 poppedOutPosition;
    private Vector3 poppedInPosition;

    private void Start()
    {
        poppedOutPosition = transform.position;
        poppedInPosition = transform.position - Vector3.up * transform.localScale.y;

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
        // the following two lines calculate if the other object is in front or behind the player
        // ensures that the other object is a hazard
        var playerTag = other.gameObject.tag;
        if (playerTag == "Player" || playerTag == "Drill")
        {
            Debug.Log("Hit Hazard");
        }
    }
}
