using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RandomEvent : MonoBehaviour
{
    public BasicRange timeBetweenEvents;
    public BasicRange eventLength;

    public void Start()
    {
        StopAllCoroutines();
        StartCoroutine(Event());
    }

    IEnumerator Event()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenEvents.Evaluate());
            InitEvent();
            yield return new WaitForSeconds(eventLength.Evaluate());
            EndEvent();
        }
    }

    public abstract void InitEvent();
    public abstract void EndEvent();
}
