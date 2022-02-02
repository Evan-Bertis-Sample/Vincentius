using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeetingApollo : Interactable
{
    private DialogueEmitter emitter;
    public int dialogueFrontend = 2;

    public Sprite apolloSprite;

    DialogueManager.OnConvo OnConvoEnd;

    private void Start()
    {
        emitter = (emitter == null) ? GetComponent<DialogueEmitter>() : emitter;
    }

    public override void OnContact(GameObject player)
    {
        OnConvoEnd = new DialogueManager.OnConvo(em => {
            if (em == emitter) PostConvo();
        });

        DialogueManager.OnConversationEnd += OnConvoEnd;
    }

    public override void OnExit(GameObject player)
    {

    }

    public override void OnHold(GameObject player)
    {

    }

    private void PostConvo()
    {
        DialogueManager.OnConversationEnd -= OnConvoEnd;
        Debug.Log("Conversation Ended");
    }
}
