using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PerlinRange : Range
{
    public float noiseScale;
    private float totalTime;
    private float randomY;

    public PerlinRange(float _min, float _max, float scale)
    {
        min = _min;
        max = _max;
        noiseScale = scale;
        totalTime = 0;
        randomY = Random.Range(-float.MaxValue, float.MaxValue);
    }
    public void Update()
    {
        totalTime += Time.deltaTime;
    }
    public override float Evaluate()
    {
        float t = Mathf.PerlinNoise(totalTime * noiseScale, randomY);
        return Mathf.Lerp(min, max, t);
    }
}
