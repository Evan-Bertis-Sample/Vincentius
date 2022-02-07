using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeetingApollo : Interactable
{
    public DialogueEmitter emitter;
    public int dialogueFrontend = 2;
    public Quest apolloQuest;
    public Sprite apolloSprite;

    public Launch launchAction;

    private PlayerController controller;

    private float originalGravityScale;

    DialogueManager.OnConvo OnConvoEnd;

    private void Start()
    {
        emitter = (emitter == null) ? GetComponent<DialogueEmitter>() : emitter;
        emitter.enabled = true;
    }

    public override void OnContact(GameObject player)
    {
        if (apolloQuest.completed) return;

        OnConvoEnd = new DialogueManager.OnConvo(em => {
            if (em == emitter) StartCoroutine(PostConvo());
        });

        DialogueManager.OnConversationEnd += OnConvoEnd;

        controller = player.GetComponent<PlayerController>();
        controller.rb.velocity = Vector2.zero;
        originalGravityScale = controller.rb.gravityScale;
        controller.rb.gravityScale = 0;
        controller.canMove = false;
    }

    public override void OnExit(GameObject player)
    {
        if (apolloQuest.completed) return;
    }

    public override void OnHold(GameObject player)
    {

    }

    private IEnumerator PostConvo()
    {
        DialogueManager.OnConversationEnd -= OnConvoEnd;
        DialogueManager.Instance.RemoveEmitter(emitter);

        apolloQuest.completed = true;
        while(launchAction.CheckAction(controller) == false && launchAction.CheckActionAfterInit(controller) == false)
        {
            Debug.Log("Waiting");
            yield return new WaitForFixedUpdate();
        }
        controller.canMove = true;
        launchAction.originalGravityScale = originalGravityScale;
        Debug.Log("Launched");

        gameObject.SetActive(false);
    }
}
