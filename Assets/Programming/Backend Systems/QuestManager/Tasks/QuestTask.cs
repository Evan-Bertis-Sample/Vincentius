using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestTask : ScriptableObject
{
    public string taskName;
    [TextArea] public string taskDescription;
    public bool completed;

    public void BeginTask()
    {
        completed = false;
    }

    public bool EvaluateTask()
    {
        Debug.Log("Evaluating Task");
        if (completed) return true;

        if (Evaluate())
        {
            completed = true;
            return true;
        }
        else return false;
    }

    protected abstract bool Evaluate();
}
