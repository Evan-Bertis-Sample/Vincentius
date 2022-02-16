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

    public Sprite speechBubbleSprite;
    public string dialogueSortingLayer = "UI";
    public Material spriteMaterial;
    public float speechBubbleDisplayTime = 0.25f;
    public float speechBubbleBobAmplitude = 0.1f;
    public float speechBubbleBobSpeed = 0.75f;

    public List<DialogueFrontend> frontends;

    public delegate void OnConvo(DialogueEmitter emitter);

    public static OnConvo OnConversationStart;
    public static OnConvo OnConversationEnd;

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
        LevelManager.OnSceneChange += newScene => possibleEmitters = possibleEmitters.Where(e => e != null).ToList(); //Remove missing references
    }

    private void LateUpdate()
    {
        nextActionInit = false;
        leaveActionInit = false;
    }

    public DialogueFrontend GetFrontend(int index)
    {
        if(index > frontends.Count - 1) return frontends[0];

        return frontends[index];
    }

    public void Reset()
    {
        visitedDialogues.Clear();
        currentDialogue = null;
        possibleEmitters.Clear();
    }

    public DialogueEmitter FindEmitter(Vector3 pos)
    {
        possibleEmitters = possibleEmitters.Where(e => e != null).ToList();
        if (possibleEmitters.Count == 0) return null;
        foreach(DialogueEmitter em in possibleEmitters)
        {
            if (em.active)
            {
                return em;
            }
        }

        List<DialogueEmitter> plausible = possibleEmitters.Where(e => e.cannotBeObstructed == false).ToList(); //Narrows down list to objects that do not need to be raycast
        List<DialogueEmitter> toRaycast = possibleEmitters.Except(plausible).ToList(); //Narrows down list to objects that need to be raycasted
        plausible.AddRange(toRaycast.Where((e) =>
            {
                RaycastHit2D collisionData = Physics2D.Linecast(pos, e.transform.position, collisionCheckMask);
                Color lineColor = (collisionData.collider == null) ? Color.green : Color.green;
                Debug.DrawLine(pos, e.transform.position, lineColor);
                return (collisionData.collider == null);
            }
        ).ToList()); //Adds all emitters that pass the collision test to the plausible list

        if (plausible.Count == 0) return null;
        if (plausible.Count == 1) return plausible[0];

        float smallestSqrDistance = float.MaxValue;
        DialogueEmitter final = plausible[0];
        foreach (DialogueEmitter e in plausible)
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
        if (!possibleEmitters.Contains(emitter)) return;
        StartCoroutine(StartConversationCoroutine(emitter));
    }

    private IEnumerator StartConversationCoroutine(DialogueEmitter emitter)
    {
        currentDialogue = emitter.GetStartingDialogue();
        emitter.active = true;
        emitter.frontend.StartConversation(emitter);
        OnConversationStart?.Invoke(emitter);
        Dialogue end = emitter.GetEndingDialogue();

        if (emitter.disablePlayerMovement) LevelManager.Instance.player.GetComponent<PlayerController>().controllerActive = false;
        while (currentDialogue != null)
        {
            if (!visitedDialogues.Contains(currentDialogue)) visitedDialogues.Add(currentDialogue);
            currentDialogue.selectedResponse = FindFirstResponse(currentDialogue);

            string noCommandDialogue = TextUnwrapper.Instance.RemoveCommands(currentDialogue.text);
            emitter.frontend.displayedText = "";
            emitter.frontend.noCommandText = noCommandDialogue;
            emitter.frontend.OnDisplayStart(emitter, currentDialogue);
            bool flagFinished = false;

            while (possibleEmitters.Contains(emitter))
            {
                //Loop while dialogue is displaying
                //Check whether the player wants to skip or move onto the next dialogue
                if (nextActionInit || leaveActionInit)
                {
                    if (emitter.frontend.displayedText != noCommandDialogue) 
                    {
                        Debug.Log("Requested to skip dialogue");
                        emitter.frontend.displayedText = noCommandDialogue;
                    }
                    else 
                    {
                        Debug.Log("Requested to move onto next dialogue");
                        break;
                    }
                }

                //Check when the dialogue is finished displaying
                if (emitter.frontend.displayedText == noCommandDialogue)
                {
                    //It is fully displayed
                    SelectResponse(currentDialogue);
                    List<NextDialogue> next = currentDialogue.GetResponses();
                    if (!flagFinished)
                    {
                        //Call the frame the text is finished displaying
                        emitter.frontend.OnDisplayEnd(emitter, currentDialogue);
                        if (next != null)emitter.frontend.OnResponseStart(next, currentDialogue.selectedResponse);
                        flagFinished = true;
                    }
                    else
                    {
                        if (next != null) emitter.frontend.OnResponseUpdate(next, currentDialogue.selectedResponse);
                    }
                }
                else
                {
                    emitter.frontend.OnDisplayUpdate(emitter, currentDialogue);
                }

                //Continue if the dialogue has finished, and if the dialogue is programmed to
                if(currentDialogue.continueOnComplete && flagFinished) break;


                yield return null;
            }

            if (!flagFinished) emitter.frontend.OnDisplayEnd(emitter, currentDialogue); //If the dialogue never finished, make sure to exectue this
            //Switch to the next dialogue
            Dialogue nextDialogue = (currentDialogue.selectedResponse == null) ? null : currentDialogue.selectedResponse.next;

            if (nextDialogue == null && currentDialogue != end)
            {
                //No more next dialogues
                //Debug.Log("Reached End");
                nextDialogue = end;
            }

            currentDialogue = nextDialogue;

            if (leaveActionInit && currentDialogue != end && currentDialogue != null) currentDialogue = end;

            //If the player leaves the conversation
            if (!possibleEmitters.Contains(emitter)) break;
            yield return null;
        }
        if (emitter.disablePlayerMovement) LevelManager.Instance.player.GetComponent<PlayerController>().controllerActive = true;
        emitter.frontend.EndConversation(emitter);
        OnConversationEnd?.Invoke(emitter);
        emitter.active = false;
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

        if (responses == null) return null;

        return dialogue.GetResponses().FirstOrDefault();
    }

    public void AddEmitter(DialogueEmitter emitter)
    {
        if (possibleEmitters.Contains(emitter)) return;
        possibleEmitters.Add(emitter);
    }

    public void RemoveEmitter(DialogueEmitter emitter)
    {
        if (!possibleEmitters.Contains(emitter)) return;
        possibleEmitters.Remove(emitter);
    }
}
