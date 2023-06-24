using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float launchSpeed;
    private Vector3 launchDirection;

    public void Launch(Vector3 direction, float speed)
    {
        launchDirection = direction.normalized;
        launchSpeed = speed;

        // Start a coroutine to move the projectile over time
        StartCoroutine(MoveProjectile());
    }

    IEnumerator MoveProjectile()
    {
        float distanceTraveled = 0f;

        while (distanceTraveled < launchSpeed)
        {
            float moveDistance = launchSpeed * Time.deltaTime;
            transform.position += launchDirection * moveDistance;
            distanceTraveled += moveDistance;

            yield return null;
        }

        // Destroy the projectile when it has reached its maximum range or hit something
        Destroy(gameObject);
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
