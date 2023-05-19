using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Blackboard : MonoBehaviour
{
    public float timeOfDay;
    public TextMeshProUGUI clock;
    public Stack<GameObject> patrons = new Stack<GameObject>();
    public int openTime = 6;
    public int closedTime = 20;


    static Blackboard instance;
    public static Blackboard Instance
    {
        get
        {
            if(!instance)
            {
                Blackboard[] blackboards = GameObject.FindObjectsOfType<Blackboard>();
                if(blackboards != null)
                {
                    if(blackboards.Length == 1)
                    {
                        instance = blackboards[0];
                        return instance;
                    }
                }
                GameObject go = new GameObject("Blackboard", typeof(Blackboard));
                instance = go.GetComponent<Blackboard>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }

        set
        {
            instance = value as Blackboard;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateClock());
    }

    IEnumerator UpdateClock()
    {
        while(true)
        {
            timeOfDay++;
            if(timeOfDay > 23) { timeOfDay = 0; }
            clock.text = timeOfDay + ":00";
            if(timeOfDay == closedTime) { patrons.Clear();}
            yield return new WaitForSeconds(1);
        }
    }

    public bool RegisterPatron(GameObject p)
    {
        patrons.Push(p);
        return true;
    }

    public void DeregisterPatron()
    {
        patrons.Clear();
    }
}
