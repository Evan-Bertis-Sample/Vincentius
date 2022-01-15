using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueInteractor : MonoBehaviour
{
    public InputAction interactAction;
    public bool interactActionInit;

    public DialogueEmitter selectedEmitter;

    private void Awake() 
    {
        interactAction.Enable();
        interactAction.started += context => interactActionInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        selectedEmitter = DialogueManager.Instance.FindEmitter(transform.position);

        if(interactActionInit)
        {
            DialogueManager.Instance.StartConversation(selectedEmitter);
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
