using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpawner : MonoBehaviour
{
    public GameObject particlePrefab;
    public float onDuration = 2f; // Duration for particles to be enabled
    public float offDuration = 1f; // Duration for particles to be disabled
    [SerializeField] public Transform spawnPosition;

    private float timer = 0f;
    private bool isParticlesEnabled = true;

    private void Start()
    {
        particlePrefab.transform.position = spawnPosition.position;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (isParticlesEnabled && timer >= onDuration)
        {
            DisableParticles();
        }
        else if (!isParticlesEnabled && timer >= offDuration)
        {
            EnableParticles();
        }
    }

    private void DisableParticles()
    {
        particlePrefab.SetActive(false);
        isParticlesEnabled = false;
        timer = 0f;
    }

    private void EnableParticles()
    {
        particlePrefab.SetActive(true);
        isParticlesEnabled = true;
        timer = 0f;
    }
}
