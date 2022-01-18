using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueInteractor : MonoBehaviour
{
    public InputAction interactAction;
    public bool interactActionInit;

    public DialogueEmitter selectedEmitter;
    private DialogueEmitter lastSelectedEmitter;

    private void Awake()
    {
        interactAction.Enable();
        interactAction.started += context => interactActionInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        lastSelectedEmitter = selectedEmitter;

        if (selectedEmitter == null)
        {
            selectedEmitter = DialogueManager.Instance.FindEmitter(transform.position);
        }
        else if (!selectedEmitter.active) 
        {
            selectedEmitter = DialogueManager.Instance.FindEmitter(transform.position);
        }

        if (selectedEmitter != null)
        {
            if (!selectedEmitter.active)
            {
                bool right = (selectedEmitter.transform.position.x - transform.position.x <= 0);
                selectedEmitter.ShowSpeechBubble(right);
                if (interactActionInit || !selectedEmitter.requireInput)
                {
                    DialogueManager.Instance.StartConversation(selectedEmitter);
                }
            }
            else
            {
                selectedEmitter.HideSpeechBubble();
            }
        }


        if (selectedEmitter != lastSelectedEmitter)
        {
            lastSelectedEmitter?.HideSpeechBubble();
        }
    }

    private void LateUpdate()
    {
        interactActionInit = false;
    }

    private bool InteractionButtonPressed()
    {
        return (interactAction.ReadValue<float>() > 0);
    }
}
