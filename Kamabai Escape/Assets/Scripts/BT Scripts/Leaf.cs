using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : Node
{
    public delegate Status Tick(); //Enumberable with whatever you try to pass in
    public Tick ProcessMethod;

    public delegate Status TickM(int val); //Tick Multiple
    public TickM ProcessMethodM; //ProcessMethod Multiple

    public int index;
    public Leaf() { }

    public Leaf(string n, Tick pm)
    {
        name = n;
        ProcessMethod = pm;
    }

    public Leaf(string n, int i, TickM pm)
    {
        name = n;
        ProcessMethodM = pm;
        index = i;
    }

    public Leaf(string n, Tick pm, int order)
    {
        name = n;
        ProcessMethod = pm;
        sortOrder = order;
    }

    public override Status Process()
    {
        Node.Status s;
        if(ProcessMethod != null)
            s = ProcessMethod();
        else if(ProcessMethodM != null)
             s = ProcessMethodM(index);
        else
            s = Status.FAILURE;

        Debug.Log(name + "  " + s); //Checks the behavior of your agent if it was successful, running, or failing
        return s;
    }

}
