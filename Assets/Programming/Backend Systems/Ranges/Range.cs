using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Range
{
    public float min = 0f;
    public float max = 1f;

    public abstract float Evaluate();
}
