using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tutorial Card", menuName = "Tutorial")]
public class TutorialCard : ScriptableObject
{
    public new string name;
    public string description;

    public Sprite artwork;

}
