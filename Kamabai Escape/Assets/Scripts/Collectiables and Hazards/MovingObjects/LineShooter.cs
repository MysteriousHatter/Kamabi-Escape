using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//I don't know if I chose the best name for this, but this script makes enemies shoot directly left of their current position
public class LineShooter : MonoBehaviour
{
    public GameObject bullet;
    public Transform objectSpawnPoint;
    public float projectileSpeed = 10f;
    private bool ableToFire = true;

    private Dictionary<Vector3, Quaternion> directionMap;

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (ableToFire == true)
        {
            StartCoroutine(FireWeapons());
        }
    }

    IEnumerator FireWeapons()
    {
        ableToFire = false;
        // Calculate the shooting direction based on the rotation
        Vector3 shootingDirection = transform.up;

        // Instantiate the projectile
        GameObject projectile = Instantiate(bullet, objectSpawnPoint.position, Quaternion.identity);

        // Move the projectile in the shooting direction over time
        EnemyBullet projectileController = projectile.GetComponent<EnemyBullet>();
        projectileController.Launch(shootingDirection, projectileSpeed);
        yield return new WaitForSeconds(2f);
        ableToFire = true;
    }
}