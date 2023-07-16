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

    public Player theKid { get; set; }
    [SerializeField] private GameObject pickUpPrefab;
    //private float waitTime = 5f
    //private float waitCounter = 8f;
    //private bool isWaiting = true;
    //We'll change to a getter or setter
    public bool isStunned {  get; set; }
    Transform waypointPoint;
   // public GameObject theKid;
    GameObject pickup;
    public float maxAngle = 45;
    public float maxDistance = 20f;
    public float chasingDistance = 10f;
    public float stunMeter = 20f;
    private float stunMeterMin = 40;
    private float stuneMeterPlaceHolder;
    [SerializeField] private float stunMeterDecrement = 5f;
    public float decreaseRate = 1f;
    public float decreaseMultiplier = 0.5f;
    public GameObject droppingPoint;
    private int currentWaypointIndex = 0;
    // Start is called before the first frame update

    private void Awake()
    {
        isStunned = false;
    }
    public override void Start()
    {
        waypointPoint = this.transform;
        base.Start();
        theKid = FindObjectOfType<Player>();
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

    private void Update()
    {
        Debug.Log("Distance to target" + distanceToTarget);
    }

    private Node.Status AttackedByAGrapple()
    {
       Node.Status stunned = IsStunned();
        if(stunned == Node.Status.SUCCESS)
        {
            Debug.Log("We are stunned");
            pickup = null;
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
        pickup = null;
        Debug.Log("Current stun meter: " + stunMeter);
        yield return new WaitForSeconds(stunMeter);
        Debug.Log("We are done decreasing are stun");
        stunMeter = stuneMeterPlaceHolder - stunMeterDecrement;
        if (stunMeter <= 0)
        {
            stunMeter = stunMeterMin;
        }
        isStunned = false;
        theKid = FindObjectOfType<Player>();
    }

    public Node.Status CanSeeKid()
    {
        if (theKid != null)
        {
            Node.Status criminalStatus = CaneSee(theKid.transform.localPosition, theKid.tag, maxDistance, maxAngle);
            Debug.Log("Can see the kid " + criminalStatus);
            return criminalStatus;
        }
        return Node.Status.FAILURE;
    }

    public Node.Status ChaseTheKid()
    {
        GameObject player = theKid.gameObject;
        Node.Status chaseTheCriminal = Chase(player.transform.localPosition, chasingDistance);
        Debug.Log("Distance to target" + distanceToTarget);
        if(!isStunned && distanceToTarget < 1.4f)
        {
           
            pickup = player;
            pickup.GetComponent<Player>().enemyPrefab = this.gameObject;
            pickup.transform.SetParent(pickUpPrefab.gameObject.transform);
            pickup.transform.localPosition = Vector3.zero;
            pickup.transform.localRotation = Quaternion.identity;
            pickup.GetComponent<Rigidbody>().isKinematic = true;
            pickup.GetComponent<Collider>().enabled = false;
            pickup.GetComponent<Player>().capturedEvent.Invoke();
        }
        Debug.Log("We are picking up the kid");
        return chaseTheCriminal;
    }


    private Node.Status DropTheKid()
    {
        Debug.Log("We are dropping off the kid");
        Node.Status success = Node.Status.FAILURE;
        if (pickup != null) { 
            Node.Status s = GoToLocation(droppingPoint.transform.position);
            if (s == Node.Status.SUCCESS)
            {
                if (pickup != null)
                {
                    Debug.Log("We are are dropping the kid");
                    Vector3 dropPosition = droppingPoint.transform.position + new Vector3(0f, 0f, 10f);
                    pickup.transform.position = dropPosition;
                    // pickup.SetActive(false);
                    theKid.PlayerDeath();
                    pickup.transform.SetParent(null);
                    pickup.GetComponent<Rigidbody>().isKinematic = false;
                    pickup.GetComponent<Collider>().enabled = true;
                    success = s;
                    pickup = null;
                }
            }
        }
        else
        {
            return Node.Status.FAILURE;
        }
        return success;
    }


    public IEnumerator WaitBeforeMoving()
    {
        //This will be were we will put our animation for the enemy
        agent.speed = 0f;
        agent.angularSpeed = 0f;
        agent.acceleration = 0f;
        //canMove = false;
        //animator.SetBool("Move", canMove);
        yield return new WaitForSeconds(1f);
        //animator.SetBool("Move", canMove);
        //canMove = true;
        agent.speed = 12f;
        agent.angularSpeed = 120f;
        agent.acceleration = 8f;

    }

}
