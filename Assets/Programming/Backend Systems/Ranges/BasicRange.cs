using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicRange : Range
{
    public override  float Evaluate()
    {
        return (Random.Range(min, max));
    }
}
