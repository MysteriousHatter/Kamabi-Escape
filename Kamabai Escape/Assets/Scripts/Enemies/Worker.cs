using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : BTAgent
{
    public GameObject office;
    GameObject patron;
    public override void Start()
    {
        base.Start();

        Selector worker = new Selector("Work At Meuseum");
        Sequence patronSequence = new Sequence("Work With The Patron");
        Leaf patronStillWaiting = new Leaf("Waiting For Patron", PatronWaiting);
        Leaf allocatePatron = new Leaf("Allocate a patron", AllocatePatron);
        Leaf goToPatron = new Leaf("Give Patron Ticket", GoToPatron);
        Leaf goToOffice = new Leaf("Head To The Office", GoToOffice);

        BehaviourTree waiting = new BehaviourTree();
        waiting.AddChild(patronStillWaiting);
        DepSequence moveToPatron = new DepSequence("Moving To Patron", waiting, agent);
        moveToPatron.AddChild(goToPatron);

        patronSequence.AddChild(allocatePatron);
        patronSequence.AddChild(moveToPatron);
        worker.AddChild(patronSequence);
        worker.AddChild(goToOffice);

        tree.AddChild(worker);
    }

    public Node.Status PatronWaiting()
    {
        if (patron == null) return Node.Status.FAILURE;
        if (patron.GetComponent<PatreonBehaviour>().isWaiting == true)
        {
            return Node.Status.SUCCESS;
        }
        else
        {
            return Node.Status.FAILURE;
        }
    }

    public Node.Status AllocatePatron()
    {
        if (Blackboard.Instance.patrons.Count == 0) { return Node.Status.FAILURE; }
        patron = Blackboard.Instance.patrons.Pop();
        if(patron == null) { return Node.Status.FAILURE; }
        else { return Node.Status.SUCCESS; }
    }

    public Node.Status GoToPatron()
    {
        if (patron == null) return Node.Status.FAILURE;
        Node.Status s = GoToLocation(patron.transform.position);
        if(s == Node.Status.SUCCESS)
        {
            patron.GetComponent<PatreonBehaviour>().ticket = true;
            patron = null;
            //Blackboard.Instance.DeregisterPatron();
        }
        return s;
    }

    public Node.Status GoToOffice()
    {
        Node.Status s = GoToLocation(office.transform.position);
        patron = null;
        return s;
    }
}
