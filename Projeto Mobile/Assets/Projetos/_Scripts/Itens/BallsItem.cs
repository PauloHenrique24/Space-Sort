using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Balls", menuName = "Scriptable Objects/Balls")]
public class BallsItem : ScriptableObject
{
    public string name_;
    public List<BallColor> ballsColor;
}