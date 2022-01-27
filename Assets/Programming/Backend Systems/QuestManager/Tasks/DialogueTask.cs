using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Task/Dialogue Task")]
public class DialogueTask : QuestTask
{
    public List<Dialogue> mustSeeDialogue = new List<Dialogue>();

    protected override bool Evaluate()
    {
        if (mustSeeDialogue.Count == 0) return true;

        return DialogueManager.Instance.visitedDialogues.Intersect(mustSeeDialogue).Count() == mustSeeDialogue.Count();
    }
}
