using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Time of Day")]
public class TimeOfDay : ScriptableObject
{
    public float fadeAmount = 0f;
    public Color baseSkyColor;
    public Color shadowSkyColor;
}
