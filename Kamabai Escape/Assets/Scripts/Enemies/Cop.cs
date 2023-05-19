using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cop : BTAgent
{
    public float speed = 15f;
    public List<GameObject> patrolPointsList;
    Transform targetPostion;
    private float waitTime = 1f;
    private float waitCounter = 0f;
    private bool waiting = true;
    Transform waypointPoint;
    public GameObject Criminal;
    public float maxAngle = 45;
    public float maxDistance = 20f;
    public float chasingDistance = 10f;
    // Start is called before the first frame update
    public override void Start()
    {
        waypointPoint = this.transform;
        base.Start();
        Sequence patrolMeuseum = new Sequence("Patrolling the Meuseum");
        PSelector patrolPoints = new PSelector("Patrol Next Points");
        Leaf waypointLeaf;
        
        for (int i = 0; i < patrolPointsList.Count; i++)
        {
            waypointLeaf = new Leaf("Waypoint: " + patrolPointsList[i].gameObject.name, i, GoToTheNextWaypoint);
            patrolMeuseum.AddChild(waypointLeaf);

        }

        Sequence chase = new Sequence("Chasing the criminals");
        Leaf seesCriminal = new Leaf("Sees the criminal", CanSeeCriminal);
        Leaf ChasesTheCriminal = new Leaf("Chase the cops", ChaseTheCriminal);
        chase.AddChild(seesCriminal);
        chase.AddChild(ChasesTheCriminal);


        Inverter CantSeeCriminal = new Inverter("Can't see Criminal");
        CantSeeCriminal.AddChild(seesCriminal);

        BehaviourTree chaseConditions = new BehaviourTree();
        Sequence conditions = new Sequence("Chasing Conditions");
        conditions.AddChild(CantSeeCriminal);
        chaseConditions.AddChild(conditions);
        DepSequence patrol = new DepSequence("Chase those criminals", chaseConditions, agent);
        patrol.AddChild(patrolMeuseum);

        Selector beCop = new Selector("Be a cop");
        beCop.AddChild(patrol);
        beCop.AddChild(chase);



        tree.AddChild(beCop);

    }


    public Node.Status GoToTheNextWaypoint(int index)
    {
        waypointPoint = patrolPointsList[index].gameObject.transform;
        Node.Status s = GoToLocation(waypointPoint.transform.position);
        return s;
    }

    public Node.Status CanSeeCriminal()
    {
        Node.Status criminalStatus = CaneSee(Criminal.transform.position, Criminal.tag, maxDistance, maxAngle);
        return criminalStatus; 
    }

    public Node.Status ChaseTheCriminal()
    {
        Node.Status chaseTheCriminal = Chase(Criminal.transform.position, chasingDistance);
        return chaseTheCriminal;
    }

}
