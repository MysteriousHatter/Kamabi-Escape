using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] WaveConfig waveConfig;
    List<Transform> waypoints;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] int waypointIndex = 0;


    [SerializeField] public bool canMove = true;
    [SerializeField] public bool hitByGrapple;
    [SerializeField] int whenToGo = 4;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        waypoints = waveConfig.GetWayPoints();
        transform.position = waypoints[0].transform.position;
        animator = gameObject.GetComponent<Animator>();
        moveSpeed = waveConfig.GetMoveSpeed();
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        if (this.canMove == true && hitByGrapple) { MOve(); }
    }

    private void MOve()
    {
        if (waypointIndex <= waypoints.Count - 1)
        {
            var targetPosition = waypoints[waypointIndex].transform.position;
            Debug.Log("The waypoints's name " + waypoints[waypointIndex].gameObject.name);
            var movementThisFrame = waveConfig.GetMoveSpeed() * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementThisFrame);

            if (transform.position == targetPosition)
            {
                StartCoroutine(WaitToGo(whenToGo));
                waypointIndex++;
            }

        }
        else
        {
            waypointIndex = 0;
        }
    }

    IEnumerator WaitToGo(int seconds)
    {
        this.canMove = false;
        //animator.SetBool("toAttack", this.canMove);
        int counter = seconds;
        while (counter > 0)
        {
            yield return new WaitForSeconds(1);
            counter--;
        }

        this.canMove = true;
       // animator.SetBool("toAttack", this.canMove);
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Player"))
        {
            // Store the player's current local scale
            Vector3 playerScale = collisionInfo.gameObject.transform.localScale;
        }
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Player"))
        {
            // Unparent the player object from the MovingPlatform
            collisionInfo.gameObject.transform.parent = null;
        }
    }
}
