using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public List<Dialogue> visitedDialogues = new List<Dialogue>();
    public Dialogue currentDialogue;

    public List<DialogueEmitter> possibleEmitters;
    public LayerMask collisionCheckMask;

    public InputAction nextAction;
    public bool nextActionInit;
    public InputAction leaveAction;
    public bool leaveActionInit;

    private bool stickDown;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);

        nextAction.Enable();
        leaveAction.Enable();
        nextAction.started += context => nextActionInit = true;
        leaveAction.started += context => leaveActionInit = true;
    }
    private void LateUpdate() 
    {
        nextActionInit = false;
        leaveActionInit = false;
    }

    public DialogueEmitter FindEmitter(Vector3 pos)
    {
        if (possibleEmitters.Count == 0) return null;

        List<DialogueEmitter> plausible = possibleEmitters.Where(e => e.cannotBeObstructed == false).ToList(); //Narrows down list to objects that do not need to be raycast
        List<DialogueEmitter> toRaycast = possibleEmitters.Except(plausible).ToList(); //Narrows down list to objects that need to be raycasted
        plausible.Concat(toRaycast.Where((e) =>
            {
                RaycastHit2D collisionData = Physics2D.Linecast(pos, e.transform.position, collisionCheckMask);
                Color lineColor = (collisionData.collider == null) ? Color.green : Color.green;
                Debug.DrawLine(pos, e.transform.position, lineColor);
                return (collisionData.collider == null);
            }
        ).ToList()); //Adds all emitters that pass the collision test to the plausible list

        if(plausible.Count == 0) return null;
        if(plausible.Count == 1) return plausible[0];

        float smallestSqrDistance = float.MaxValue;
        DialogueEmitter final = plausible[0];
        foreach(DialogueEmitter e in plausible)
        {
            float sqrDistance = (e.transform.position - pos).sqrMagnitude;
            if (sqrDistance < smallestSqrDistance)
            {
                smallestSqrDistance = sqrDistance;
                final = e;
            }
        }

        return final;
    }

    public void StartConversation(DialogueEmitter emitter)
    {
        StartCoroutine(StartConversationCoroutine(emitter));
    }

    private IEnumerator StartConversationCoroutine(DialogueEmitter emitter)
    {
        currentDialogue = emitter.GetStartingDialogue();
        emitter.frontend.StartConversation(emitter);

        while(currentDialogue != null)
        {
            if (!visitedDialogues.Contains(currentDialogue)) visitedDialogues.Add(currentDialogue);
            currentDialogue.selectedResponse = FindFirstResponse(currentDialogue);
            while(!nextActionInit && !leaveActionInit && possibleEmitters.Contains(emitter))
            {
                emitter.frontend.OnDisplayDialogue(emitter, currentDialogue);
                SelectResponse(currentDialogue);
                yield return null;
            }

            if (leaveActionInit || !possibleEmitters.Contains(emitter)) break;
            yield return null;
            
            currentDialogue = (currentDialogue.selectedResponse == null) ? null : currentDialogue.selectedResponse.next;
        }

        currentDialogue = emitter.GetEndingDialogue();
        emitter.frontend.EndConversation(emitter);
    }

    public void SelectResponse(Dialogue dialogue)
    {
        float dir = Input.GetAxisRaw("Vertical");
        if (dir != 0)
        {
            if (!stickDown) 
            {
                CycleResponses(dialogue, dir);
            }

            stickDown = true;
        }
        else
        {
            stickDown = false;
        }
    }

    private static void CycleResponses(Dialogue dialogue, float dir)
    {
        List<NextDialogue> responses = dialogue.GetResponses();

        if (responses.Count == 1) return;

        NextDialogue selectedResponse = dialogue.selectedResponse;
        int index = responses.IndexOf(selectedResponse);

        if (dir >= 0)
        {
            if (index == 0)
            {
                index = responses.Count - 1;
            }
            else index--;
        }
        else if (dir <= 0)
        {
            if (index == responses.Count - 1)
            {
                index = 0;
            }
            else index++;
        }

        dialogue.SelectResponse(responses[index]);
    }

    public NextDialogue FindFirstResponse(Dialogue dialogue)
    {
        List<NextDialogue> responses = dialogue.GetResponses();

        if(responses == null) return null;

        return dialogue.GetResponses().FirstOrDefault();
    }

    public void AddEmitter(DialogueEmitter emitter)
    {
        possibleEmitters.Add(emitter);
    }

    public void RemoveEmitter(DialogueEmitter emitter)
    {
        possibleEmitters.Remove(emitter);
    }
}
