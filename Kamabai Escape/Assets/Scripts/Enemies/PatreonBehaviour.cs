using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatreonBehaviour : BTAgent
{
    public GameObject home;
    public GameObject frontdoor;

    public List<Item> art;
    public Dictionary<int, Item> art_dict = new Dictionary<int, Item>();


    [Range(0, 1000)]
    public int boerdMeter = 800;

    public bool ticket = false;
    public bool isWaiting = false;

    Leaf goToFrontDoor;
    Leaf randomItem;

    RSelector randomObject;
    bool isHome;
    public int boredValue = 150;


    // Start is called before the first frame update
    public override void Start()
    {
        isHome = true;
        for (int i = 0; i < art.Count; i++)
        {
            art_dict.Add(i, art[i]);
        }

        base.Start();
        Sequence viewArt = new Sequence("Seeing art conditions");
        Leaf isMeuseumOpen = new Leaf("Is Meuseum Open?", IsOpen);
        Leaf hasGotBored = new Leaf("Is really Bored", CanWeGoToMeseum);

        //PSelector opendoor = new PSelector("Open Door");
        goToFrontDoor = new Leaf("Go To Frontdoor", GoToFrontDoor);
        Leaf goToHome = new Leaf("Go To Home", GoToHome);

        randomObject = new RSelector("Select some random painting");
        foreach (Item pickup in art_dict.Values)
        {
            randomItem = new Leaf("Pickup " + pickup.gameObject.name, pickup.id, LookAtRandomObject);
            randomObject.AddChild(randomItem);
        }

        //opendoor.AddChild(goToFrontDoor);

        viewArt.AddChild(isMeuseumOpen);
        viewArt.AddChild(hasGotBored);
        viewArt.AddChild(goToFrontDoor);

        //Getting our ticket
        Leaf noTicket = new Leaf("Wait for Ticket", NoTicket);
        Leaf isWaiting = new Leaf("Waiting for Worker", IsWaiting);
        BehaviourTree waitForTicket = new BehaviourTree();
        waitForTicket.AddChild(noTicket);
        Loop getTicket = new Loop("Ticket", waitForTicket);
        getTicket.AddChild(isWaiting);
        viewArt.AddChild(getTicket);

        BehaviourTree whileBored = new BehaviourTree();
        whileBored.AddChild(hasGotBored);
        Loop loopingAtArt = new Loop("Look At Art", whileBored);

        loopingAtArt.AddChild(randomObject);
        //viewArt.AddChild(randomObject);
        viewArt.AddChild(loopingAtArt);

        viewArt.AddChild(goToHome);

        BehaviourTree galleryOpenCondition = new BehaviourTree();
        galleryOpenCondition.AddChild(isMeuseumOpen);
        DepSequence bePatron = new DepSequence("Be An Art Patron", galleryOpenCondition, agent);
        bePatron.AddChild(viewArt);

        Selector viewArtWithFallback = new Selector("View Art with Fallback");
        viewArtWithFallback.AddChild(bePatron);
        viewArtWithFallback.AddChild(goToHome);

        tree.AddChild(viewArtWithFallback);

        whileBored.PrintTree();

    }


    public Node.Status CanWeGoToMeseum()
    {
        Node.Status s = IsBored();
        if (s == Node.Status.FAILURE)
        {
            StartCoroutine(raisingBoredness());

        }
        return s;
    }
    public Node.Status IsBored()
    {
        if (boerdMeter < 100)
            return Node.Status.FAILURE;
        return Node.Status.SUCCESS;
    }


    public Node.Status LookAtRandomObject(int randIndex)
    {

        GameObject item = art_dict[randIndex].gameObject;
        if (!item.activeSelf) { return Node.Status.FAILURE; }
        Node.Status s = GoToLocation(item.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            isHome = false;
            boerdMeter = Mathf.Clamp(boerdMeter - boredValue, 0, 1000);
            // StopCoroutine(raisingBoredness());
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

    public Node.Status GoToHome()
    {
        Debug.Log("Go home");
        Node.Status s = GoToLocation(home.transform.position);
        isWaiting = false;
        return s;
    }

    IEnumerator raisingBoredness()
    {
        while (isHome)
        {
            boerdMeter = Mathf.Clamp(boerdMeter + boredValue, 0, 1000);
            yield return new WaitForSeconds(Random.RandomRange(1, 5));
        }
    }



    public Node.Status NoTicket()
    {
        if (ticket || IsOpen() == Node.Status.FAILURE)
        {
            return Node.Status.FAILURE;
        }
        else { return Node.Status.SUCCESS; }
    }

    public Node.Status IsWaiting()
    {
        if (Blackboard.Instance.RegisterPatron(this.gameObject))
        {
            isWaiting = true;
            return Node.Status.SUCCESS;
        }
        else
        {
            return Node.Status.FAILURE;
        }

    }
}



