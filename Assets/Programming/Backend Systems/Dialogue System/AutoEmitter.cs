using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoEmitter : DialogueEmitter
{
    public override void InitEmitter()
    {
        requireInput = false;
        DialogueManager.Instance.AddEmitter(this);
        DialogueManager.Instance.StartConversation(this);
        DialogueManager.OnConversationEnd += emitter =>
        {
            if (emitter == this)
            {
                DialogueManager.Instance.RemoveEmitter(this);
            }
        };
    }
}
