using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Guard : BTAgent
{
    public float speed = 15f;
    public List<GameObject> patrolPointsList;
    Transform targetPostion;
    //private float waitTime = 5f;
    //private float waitCounter = 8f;
    //private bool isWaiting = true;
    //We'll change to a getter or setter
    [SerializeField] private bool isStunned = false;
    Transform waypointPoint;
    public GameObject theKid;
    GameObject pickup;
    public float maxAngle = 45;
    public float maxDistance = 20f;
    public float chasingDistance = 10f;
    public float stunMeter = 20f;
    private float stuneMeterPlaceHolder;
    public float decreaseRate = 1f;
    public float decreaseMultiplier = 0.5f;
    public GameObject droppingPoint;
    private int currentWaypointIndex = 0;
    // Start is called before the first frame update
    public override void Start()
    {
        waypointPoint = this.transform;
        base.Start();
        stuneMeterPlaceHolder = stunMeter;
        Sequence patrolHouse = new Sequence("Patrolling the house");
        PSelector patrolPoints = new PSelector("Patrol Next Points");
        Leaf waypointLeaf;
        //Is Stunned
        Leaf stunnedByGrapple = new Leaf(name + "Is Stunned by a grapple", AttackedByAGrapple);

        // Find the last known waypoint index
        int startingWaypointIndex = Mathf.Clamp(currentWaypointIndex, 0, patrolPointsList.Count - 1);
        for (int i = 0; i < patrolPointsList.Count; i++)
        {
            waypointLeaf = new Leaf(name + "Waypoint: " + patrolPointsList[i].gameObject.name, i, GoToTheNextWaypoint);
            patrolHouse.AddChild(waypointLeaf);

        }

        Sequence chase = new Sequence("Chasing the kid");
        Leaf seesKid = new Leaf(name + "Sees the kid", CanSeeKid);
        Leaf ChasesTheKid = new Leaf(name + "Chase the kid", ChaseTheKid);
        Leaf dropTheKid = new Leaf(name + "Drop the kid", DropTheKid);
        chase.AddChild(seesKid);
        chase.AddChild(ChasesTheKid);
        chase.AddChild(dropTheKid);


        Inverter CantSeeKid = new Inverter("Can't see Kid");
        CantSeeKid.AddChild(seesKid);

        Inverter IsNotStunned = new Inverter("Guard is Stunned");
        IsNotStunned.AddChild(stunnedByGrapple);

        BehaviourTree chaseConditions = new BehaviourTree();
        Sequence conditions = new Sequence("Chasing Conditions");
        conditions.AddChild(CantSeeKid);
        conditions.AddChild(IsNotStunned);
        chaseConditions.AddChild(conditions);
        DepSequence patrol = new DepSequence("Chase that kid", chaseConditions, agent);
        patrol.AddChild(patrolHouse);

        BehaviourTree stunConditions = new BehaviourTree();
        Sequence conditionsForStun = new Sequence("Stun Conditions");
        conditionsForStun.AddChild(IsNotStunned);
        stunConditions.AddChild(conditionsForStun);
        DepSequence captureKid = new DepSequence("The Guard is Stunned", stunConditions, agent);
        captureKid.AddChild(chase);
       

        Selector beGuard = new Selector("Be a cop");
        beGuard.AddChild(patrol);
        beGuard.AddChild(captureKid);
        beGuard.AddChild(stunnedByGrapple);



        tree.AddChild(beGuard);

    }

    private Node.Status AttackedByAGrapple()
    {
       Node.Status stunned = IsStunned();
        if(stunned == Node.Status.SUCCESS)
        {
            Debug.Log("We are stunned");
            StartCoroutine(decreasingStun());
        }
        return stunned;
    }

    private Node.Status IsStunned()
    {
        Debug.Log("We have hit this method");
        if(isStunned) { return Node.Status.SUCCESS; }
        return Node.Status.FAILURE;
    }

    public Node.Status GoToTheNextWaypoint(int index)
    {
        currentWaypointIndex = index;
        waypointPoint = patrolPointsList[currentWaypointIndex].gameObject.transform;
        Node.Status s = GoToLocation(waypointPoint.transform.position);
        return s;
    }

    IEnumerator decreasingStun()
    {
        stunMeter -= (decreaseRate * decreaseMultiplier) * Time.deltaTime;
        Debug.Log("Current stun meter: " + stunMeter);
        yield return new WaitForSeconds(stunMeter);
        Debug.Log("We are done decreasing are stun");
        stunMeter = stuneMeterPlaceHolder;
        isStunned = false;
    }

    public Node.Status CanSeeKid()
    {
        Node.Status criminalStatus = CaneSee(theKid.transform.position, theKid.tag, maxDistance, maxAngle);
        return criminalStatus; 
    }

    public Node.Status ChaseTheKid()
    {
        GameObject player = theKid;
        Node.Status chaseTheCriminal = Chase(player.transform.position, chasingDistance);
        Debug.Log("Distance to target" + distanceToTarget);
        if(distanceToTarget < 2)
        {
            Debug.Log("Pick up the target");
            player.transform.parent = this.gameObject.transform;
            pickup = player;
        }
        // float distanceToTarget = Vector3.Distance(player.transform.position, this.transform.position);
        //Debug.Log("The current distance " + distanceToTarget);

        //if (Vector3.Distance(agent.pathEndPosition, player.transform.position) >= 2)
        //{
        //    state = ActionState.IDLE;
        //    return Node.Status.FAILURE;
        //}
        //else if (distanceToTarget < 2)
        //{
        //    state = ActionState.IDLE;
        //    player.transform.parent = this.gameObject.transform;
        //    pickup = player;
        //    return Node.Status.SUCCESS;

        //}
        //if (chaseTheCriminal == Node.Status.SUCCESS)
        //{
        //    player.transform.parent = this.gameObject.transform;
        //    pickup = player;

        //}
        return chaseTheCriminal;
    }


    private Node.Status DropTheKid()
    {
        Node.Status s = GoToLocation(droppingPoint.transform.position);
        if(s == Node.Status.SUCCESS)
        {
            if(pickup != null)
            {
                Vector3 dropPosition = droppingPoint.transform.position + new Vector3(0f, 0f, 10f);
                pickup.transform.position = dropPosition;
                pickup.SetActive(false);
                pickup = null;
            }
        }
        return s;
    }


    public IEnumerator WaitBeforeMoving()
    {
        //This will be were we will put our animation for the enemy
        agent.speed = 0f;
        agent.angularSpeed = 0f;
        agent.acceleration = 0f;
        //canMove = false;
        //animator.SetBool("Move", canMove);
        yield return new WaitForSeconds(5f);
        //animator.SetBool("Move", canMove);
        //canMove = true;
        agent.speed = 12f;
        agent.angularSpeed = 120f;
        agent.acceleration = 8f;

    }

}
