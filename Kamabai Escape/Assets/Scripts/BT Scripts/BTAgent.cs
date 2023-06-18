using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTAgent : MonoBehaviour
{
    public BehaviourTree tree;
    public NavMeshAgent agent;

    public enum ActionState { IDLE, WORKING };
    public ActionState state = ActionState.IDLE;

    public Node.Status treeStatus = Node.Status.RUNNING;

    WaitForSeconds waitForSeconds;
    Vector3 rememberedLocation;
    public float distanceToTarget;

    // Start is called before the first frame update
    public virtual void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        tree = new BehaviourTree();
        waitForSeconds = new WaitForSeconds(Random.Range(0.1f, 0.12f));
        StartCoroutine("Behave");
    }

    public Node.Status CaneSee(Vector3 target, string tag, float distance, float maxAngle)
    {
        Vector3 directionToTarget = target - this.transform.position;
        float angle = Vector3.Angle(directionToTarget, this.transform.forward);
        
        //First condtion checks if the robber is right in front of the cop 
        //Second condtion checks if the robber is at a close distance between the cop 
        if(angle <= maxAngle || directionToTarget.magnitude <= distance) //You can also have a condition where the robber can able to look in the direction of the cop, but not be able to detect him or see him
        {
            RaycastHit hitInfo;
            if(Physics.Raycast(this.transform.position, directionToTarget, out hitInfo))
            {
                Debug.Log("The target tag's info:" + tag);
                if (hitInfo.collider.gameObject.CompareTag(tag))
                {
                    Debug.Log("Chasing the robber");
                    return Node.Status.SUCCESS;
                }
                Debug.Log("The tag's info:" + hitInfo.collider.gameObject.tag);
            }
        }
        Debug.Log("Not chasing the robber");
        return Node.Status.FAILURE;
    }

    public Node.Status IsOpen()
    {
        if (Blackboard.Instance.timeOfDay < Blackboard.Instance.openTime || Blackboard.Instance.timeOfDay > Blackboard.Instance.closedTime)
            return Node.Status.FAILURE;
        return Node.Status.SUCCESS;
    }

    public Node.Status Flee(Vector3 location, float distance)
    {
        if (state == ActionState.IDLE)
        {
            rememberedLocation = this.transform.position + (transform.position - location).normalized * distance;

        }
            return GoToLocation(rememberedLocation);
        
    }

    public Node.Status Chase(Vector3 location, float distance)
    {
        if (state == ActionState.IDLE)
        {
            rememberedLocation = this.transform.position - (transform.position - location).normalized * distance;
        }
        return GoToLocation(rememberedLocation);

    }

    public Node.Status GoToLocation(Vector3 destination)
    {
        distanceToTarget = Vector3.Distance(destination, this.transform.position);
        if (state == ActionState.IDLE)
        {
            agent.SetDestination(destination);
            state = ActionState.WORKING;
        }
        else if (Vector3.Distance(agent.pathEndPosition, destination) >= 0.4f)
        {
            state = ActionState.IDLE;
            return Node.Status.RUNNING;
        }
        else if (distanceToTarget < 2)
        {
            state = ActionState.IDLE;
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }

    public Node.Status GoToDoor(GameObject door)
    {
        Node.Status s = GoToLocation(door.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            if (!door.GetComponent<Lock>().isLocked)
            {
                door.GetComponent<NavMeshObstacle>().enabled = false;
                return Node.Status.SUCCESS;
            }
            return Node.Status.FAILURE;
        }
        else
            return s;
    }

    IEnumerator Behave()
    {
        while (true)
        {
            treeStatus = tree.Process();
            yield return waitForSeconds;
        }
    }
}
