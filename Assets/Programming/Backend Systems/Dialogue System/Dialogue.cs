using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    [Header("Dialogue Options")]
    [TextArea] public string text;
    public List<NextDialogue> nextDialogues = new List<NextDialogue>();
    public NextDialogue selectedResponse;

    [Header("Dialogue Parameters")]
    public List<Dialogue> mustSeeDialogues = new List<Dialogue>();
    public List<Dialogue> anySeeDialogues = new List<Dialogue>();

    public bool CheckDialogue()
    {
        //Gets a list of what both lists have, then checks whether all of them have been seen
        bool seenRequired = DialogueManager.Instance.visitedDialogues.Intersect(mustSeeDialogues).Count() == mustSeeDialogues.Count();
        //Same thing, but it checks whether any of them have been seen
        bool seenAny = DialogueManager.Instance.visitedDialogues.Intersect(anySeeDialogues).Count() > ((anySeeDialogues.Count > 0) ? 0 : -1);
        //Debug.Log(seenRequired && seenAny);
        return (seenRequired && seenAny);
    }

    public List<NextDialogue> GetResponses()
    {
        if(nextDialogues == null) return null;
        if(nextDialogues.Count == 0) return null;
        IEnumerable<NextDialogue> possibleDialogues = nextDialogues.Where(d => d.next?.CheckDialogue() == true).ToList();
        return (possibleDialogues.Any()) ? possibleDialogues.ToList() : null;
    }

    public void SelectResponse(NextDialogue nDialogue)
    {
        selectedResponse = nextDialogues[nextDialogues.IndexOf(nDialogue)];
    }
}

[System.Serializable]
public class NextDialogue
{
    public string previewText;
    public Dialogue next;
}
