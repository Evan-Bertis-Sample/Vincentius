using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DialogueFrontend : MonoBehaviour
{
    public GameObject prefab;

    //Use task workflow to link methods asynchronously 

    public virtual void StartConversation(DialogueEmitter emitter)
    {
        Debug.Log("Start");
    }

    public virtual void OnDisplayDialogue(DialogueEmitter emitter, Dialogue dialogue)
    {
        //Debug.Log("Display");
        Debug.Log(dialogue.text);
        if (dialogue.nextDialogues.Count == 0) return;
        Debug.Log($"Selected Response: {dialogue.selectedResponse.next.text} ");
    }

    public virtual void DisplayResponses(List<NextDialogue> dialogues)
    {
        Debug.Log("Display 2");
    }
    public virtual void EndConversation(DialogueEmitter emitter)
    {
        Debug.Log("End");
    }
}
