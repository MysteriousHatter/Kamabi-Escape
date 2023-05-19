using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : BTAgent
{
    public GameObject diamond;
    public GameObject painting;
    public GameObject van;
    public GameObject backdoor;
    public GameObject frontdoor;
    public GameObject Cop;
    public float maxAngle = 45;
    public float maxDistance = 20f;
    public float runAwayDistance = 10f;

    public List<Item> art;
    public Dictionary<int, Item> art_dict = new Dictionary<int, Item>();

    GameObject pickup;

    [Range(0, 1000)]
    public int money = 800;

    Leaf goToBackDoor;
    Leaf goToFrontDoor;
    Leaf randomItem;

    RSelector randomObject;

    // Start is called before the first frame update
    public override void Start()
    {
        for(int i = 0; i < art.Count;i++)
        {
            art_dict.Add(i,art[i]);
        }


        base.Start();
        StartCoroutine(gettingPoor());

        Leaf isMeuseumOpen = new Leaf("Is Meusuem Open?", IsOpen);
        Inverter invertOpenMeuseum = new Inverter("Invert Openeing");
        invertOpenMeuseum.AddChild(isMeuseumOpen);
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond, 1);
        Leaf goToPainting = new Leaf("Go To Painting", GoToPainting, 2);
        Leaf hasGotMoney = new Leaf("Has Got Money", HasMoney);


        goToBackDoor = new Leaf("Go To Backdoor", GoToBackDoor, 2);
        goToFrontDoor = new Leaf("Go To Frontdoor", GoToFrontDoor, 1);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        PSelector opendoor = new PSelector("Open Door");
        //PSelector selectObject = new PSelector("Select Object to Steal");
        randomObject = new RSelector("Select some random painting");



       // selectObject.AddChild(goToDiamond);
        //selectObject.AddChild(goToPainting);

       
        foreach (Item pickup in art_dict.Values)
        {
            randomItem = new Leaf("Pickup " + pickup.gameObject.name, pickup.id, pickUpRandomObject);
            randomObject.AddChild(randomItem);
        }


        Sequence flee = new Sequence("Running Away from the feds");
        Leaf seesCop = new Leaf("Can See Cop", CanSeeCop);
        Leaf RunFromFeds = new Leaf("Flee from cops", FleeFromCop);

        Inverter CantSeeCop = new Inverter("Can't see cop");
        CantSeeCop.AddChild(seesCop);
        Inverter invertMoney = new Inverter("Invert Money");
        invertMoney.AddChild(hasGotMoney);

        opendoor.AddChild(goToFrontDoor);
        opendoor.AddChild(goToBackDoor);


        flee.AddChild(seesCop);
        flee.AddChild(RunFromFeds);

        //Sequence s1 = new Sequence("s1");
        //s1.AddChild(invertMoney);
        //Sequence s2 = new Sequence("s2");
        //s2.AddChild(CantSeeCop);
        //s2.AddChild(opendoor);
        //Sequence s3 = new Sequence("s3");
        //s3.AddChild(CantSeeCop);
        //s3.AddChild(randomObject);
        //Sequence s4 = new Sequence("s4");
        //s4.AddChild(CantSeeCop);
        //s4.AddChild(goToVan);

        //steal.AddChild(s1);
        //steal.AddChild(s2);
        //steal.AddChild(s3);
        //steal.AddChild(s4);
        BehaviourTree stealConditions = new BehaviourTree();
        Sequence conditions = new Sequence("Stealing Conditions");
        stealConditions.AddChild(invertOpenMeuseum);
        conditions.AddChild(CantSeeCop);
        conditions.AddChild(invertMoney);
        stealConditions.AddChild(conditions);
        DepSequence steal = new DepSequence("Steal Something",stealConditions, agent);
        //steal.AddChild(invertMoney);
        //steal.AddChild(invertOpenMeuseum);
        steal.AddChild(opendoor);
        steal.AddChild(randomObject);
        ///steal.AddChild(selectObject);
        //steal.AddChild(goToBackDoor);
        steal.AddChild(goToVan);

        //Our fallback behaviour
        Selector stealWithFallback = new Selector("Steal with Fallback");
        //stealWithFallback.AddChild(invertOpenMeuseum);
        stealWithFallback.AddChild(steal);
        stealWithFallback.AddChild(goToVan);




        Selector beThief = new Selector("Be a theif");
        beThief.AddChild(stealWithFallback);
        beThief.AddChild(flee);


        tree.AddChild(beThief);

        tree.PrintTree();

   }

    public Node.Status CanSeeCop()
    {
        Node.Status copStatus = CaneSee(Cop.transform.position,Cop.tag, maxDistance, maxAngle);
        return copStatus;
    }

    public Node.Status FleeFromCop()
    {
        Node.Status fleeFromCop = Flee(Cop.transform.position, runAwayDistance);
        return fleeFromCop;
    }

    public Node.Status HasMoney()
    {
        if(money < 500)
            return Node.Status.FAILURE;
        return Node.Status.SUCCESS;
    }

    public Node.Status GoToDiamond()
    {
        if (!diamond.activeSelf) return Node.Status.FAILURE;
        Node.Status s = GoToLocation(diamond.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            diamond.transform.parent = this.gameObject.transform;
            pickup = diamond;
        }
        return s;
    }

    public Node.Status GoToPainting()
    {
        if (!painting.activeSelf) return Node.Status.FAILURE;
        Node.Status s = GoToLocation(painting.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            painting.transform.parent = this.gameObject.transform;
            pickup = painting;
        }
        return s;
    }

    public Node.Status GoToBackDoor()
    {
        Node.Status s = GoToDoor(backdoor);
        if (s == Node.Status.FAILURE)
            goToBackDoor.sortOrder = 10;
        else
            goToBackDoor.sortOrder = 1;
        return s;
    }

    public Node.Status pickUpRandomObject(int randIndex)
    {

        GameObject item = art_dict[randIndex].gameObject;
        if (!item.activeSelf) { return Node.Status.FAILURE; }
        Node.Status s = GoToLocation(item.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            item.transform.parent = this.gameObject.transform;
            pickup = item;
        }
        return s;
    }

    public Node.Status GoToFrontDoor()
    {
        Node.Status s = GoToDoor(frontdoor);
        if (s == Node.Status.FAILURE)
            goToFrontDoor.sortOrder = 10;
        else
            goToFrontDoor.sortOrder = 1;
        return s;
    }

    public Node.Status GoToVan()
    {
        Node.Status s = GoToLocation(van.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            if (pickup != null)
            {
                money += 300;
                pickup.SetActive(false);
                pickup = null;
            }
        }
        return s;
    }

    public Node.Status IsOpen()
    {
        if (Blackboard.Instance.timeOfDay < 5 || Blackboard.Instance.timeOfDay > 22)
            return Node.Status.FAILURE;
        return Node.Status.SUCCESS;
    }


    IEnumerator gettingPoor()
    {
        while (true)
        {
            money = Mathf.Clamp(money - 20, 0, 1000);
            yield return new WaitForSeconds(Random.RandomRange(1, 5));
        }
    }

}
