using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class DialogueFrontend : MonoBehaviour
{
    [HideInInspector]
    public string displayedText;
    [HideInInspector]
    public string noCommandText;

    public abstract void StartConversation(DialogueEmitter emitter);
    public abstract void OnDisplayStart(DialogueEmitter emitter, Dialogue dialogue);
    public abstract void OnDisplayUpdate(DialogueEmitter emitter, Dialogue dialogue);
    public abstract void OnDisplayEnd(DialogueEmitter emitter, Dialogue dialogue);
    public abstract void OnResponseStart(List<NextDialogue> dialogues, NextDialogue selectedDialogue);
    public abstract void OnResponseUpdate(List<NextDialogue> dialogues, NextDialogue selectedDialogue);
    public abstract void EndConversation(DialogueEmitter emitter);
}
