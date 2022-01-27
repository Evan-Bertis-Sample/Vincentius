using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(menuName = "Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    [TextArea] public string questDescription;
    public bool completed;
    public List<QuestTask> questTasks = new List<QuestTask>();

    public void BeginQuest()
    {
        completed = false;
        foreach(QuestTask t in questTasks)
        {
            t.BeginTask();
        }
    }

    public bool EvaluateQuest()
    {
        if (completed) return true;
        Debug.Log("Evaluating Quest");
        
        foreach(QuestTask t in questTasks)
        {
            t.EvaluateTask();
        }

        if (questTasks.Where(qt => !qt.completed).Any()) return false;

        completed = true;
        return true;
    }
}
