using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    Vector3 startingPostion;
    [SerializeField] Vector3 movementVector;
    [SerializeField] [Range(0, 1)] float movementFactor;
    [SerializeField] private float period = 2f;

    // Start is called before the first frame update
    void Start()
    {
        startingPostion = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float cycles = Time.time / period;


        const float tau = Mathf.PI * 2;
        float rawSinWave = Mathf.Sin(cycles * tau);

        movementFactor = (rawSinWave + 1f) / 2f;


        Vector3 offset = movementVector * movementFactor;
        this.transform.position = startingPostion + offset;
    }
}
