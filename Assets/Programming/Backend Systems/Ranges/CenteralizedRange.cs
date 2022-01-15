using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CenteralizedRange : Range
{
    public float targetValue;
    public int cylces = 2;

    public override float Evaluate()
    {
        float v = Random.Range(min, max);

        for (int i = 0; i < cylces; i++)
        {
            float t = Random.Range(0, 1); //Find the t
            float newV = Mathf.Lerp(v, targetValue, t); //Interpolate towards the target value
            v = newV; //Set new starting point to be closer to the target value for the next cycle
        }

        return v;
    }
}
