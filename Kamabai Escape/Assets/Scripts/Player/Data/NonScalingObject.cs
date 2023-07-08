using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonScalingObject : MonoBehaviour
{
    private Vector3 initialScale;
    private Transform parentTransform;

    void Start()
    {
        initialScale = transform.localScale;
        parentTransform = transform.parent;
    }

    void Update()
    {
        if (parentTransform != null)
        {
            // Calculate the inverse scale of the parent
            Vector3 inverseScale = new Vector3(1 / parentTransform.localScale.x, 1 / parentTransform.localScale.y, 1 / parentTransform.localScale.z);

            // Adjust the child object's local scale based on the inverse scale of the parent
            transform.localScale = Vector3.Scale(initialScale, inverseScale);
        }
    }
}