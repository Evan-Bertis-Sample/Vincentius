using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Collider2D))]
public class DialogueEmitter : MonoBehaviour
{
    public string characterName;
    public Sprite characterPortrait;

    //List in order of priority
    public List<Dialogue> startingDialogues;
    public List<Dialogue> endingDialogues;

    public bool cannotBeObstructed = true;

    public DialogueFrontend frontend;

    public Dialogue GetStartingDialogue()
    {
        if(startingDialogues.Count == 0) return null;
        if(startingDialogues.Count == 1) return startingDialogues.First();

        List<Dialogue> canSee = startingDialogues.Where((d) => d.CheckDialogue() == true).ToList();
        return canSee.First();
    }

    public Dialogue GetEndingDialogue()
    {
        if(endingDialogues.Count == 0) return null;
        if(endingDialogues.Count == 1) return endingDialogues.First();

        List<Dialogue> canSee = endingDialogues.Where((d) => d.CheckDialogue() == true).ToList();
        return canSee.First();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<DialogueInteractor>())
        {
            DialogueManager.Instance.AddEmitter(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        DialogueManager.Instance.RemoveEmitter(this);
    }
}
