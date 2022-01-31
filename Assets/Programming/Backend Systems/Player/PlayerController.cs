using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public Vector3 moveInput;
    public float moveSpeed;
    public float moveSpeedMultiplier;
    public AnimationCurve accelerationCurve;
    public float timeMoving;
    public float timeSinceLastMove;
    public float accelerationTime = 1;
    public Vector2 lastFrameVelocity;

    [Header("States")]
    public PlayerState currentState;
    public PlayerState previousState;
    public float timeSinceLastAction;
    public bool OnGround;
    public float groundCheckDistance = 0.27f;
    public float groundCheckRadius = 0.2f;
    public float timeOnGround;

    [Header("Actions & Sets")]
    public List<PlayerActionSet> actionSets = new List<PlayerActionSet>();
    public PlayerActionSet currentSet;
    public PlayerAction currentAction;
    public PlayerAction lastAction;
    public float actionTimeElapsed;

    [Header("Debug")]
    public bool canMove = true;
    public bool debug = true;

    public Rigidbody2D rb;
    Camera cMain;
    public PlayerGFX playerGFX;

    public delegate void OnAction(PlayerAction action);
    public static OnAction OnPlayerAction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cMain = Camera.main;
        playerGFX = GetComponent<PlayerGFX>();
        foreach (PlayerActionSet actionSet in actionSets)
        {
            actionSet.Init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(GameStateManager.Instance.paused) return; //This is lazy but I don't care
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        HandleActions();
        Move();
        CheckGround();
        HandlePredfinedStateLogic();
        previousState = currentState;
        lastAction = currentAction;
    }

    void LateUpdate()
    {
        if(GameStateManager.Instance.paused) return; //This is lazy but I don't care
        if (currentSet != null)
        {
            foreach (PlayerAction action in currentSet.actions)
            {
                if (action == currentAction) continue;
                action.bindingTriggeredThisFrame = false; //Reset the flag
            }
        }
        lastFrameVelocity = rb.velocity;
    }

    void HandleActions()
    {
        if (actionSets == null) return;
        if (actionSets.Count == 0) return;
        FindCurrentSet();

        if (currentSet.actions == null) return;
        if (currentSet.actions.Count == 0) return;

        #region Check Current Action
        if (currentAction != null)
        {
            if (currentAction.waitForAnimationEvent && !currentAction.animationFlag) return; //Do not try to cancel the action if it hasn't even started yet
            if (currentAction.performingAction == false) currentAction.InitiateAction(this); //The action hasn't started yet because it was waiting for the animation event
            //Preliminary Check - if the player is already performing an action, perform that one until it cancels itself out
            currentAction.PerformAction(this);
            actionTimeElapsed = currentAction.timeElapsed;

            if (currentAction.CheckActionAfterInit(this))
            {
                //The action is still valid, what do we do?
                //If you can interrupt the action, allow for the algorithm to search for more actions. If not, stop it.
                if (!currentAction.interruptable) return;
            }
            else
            {
                //The action is no longer valid
                currentAction.ExitAction(this, false);
                currentAction = null;
            }
        }
        #endregion

        #region Find New Action
        //We are not performing an action - find an action we can perform
        actionTimeElapsed = 0f;
        foreach (PlayerAction action in currentSet.actions)
        {
            //Favors actions higher in the action set
            if (!action.CheckAction(this)) continue;
            if (action == currentAction) continue; //If we are performing an interruptable action, we want to ignore it

            if (currentAction != null) currentAction.ExitAction(this, true); //We have found a new action to replace the interruptable action. Exit the interruptable action.

            currentAction = action;
            if (debug) Debug.Log(currentAction.name.ToUpper() + " is a possible action");
            break;
        }

        if (currentAction == null) return;
        //Debug.Log("Performing Action: " + currentAction.name.ToUpper());

        if (currentAction.performingAction == false)
        {
            //the action has been selected but is just starting
            currentAction.InitiateAction(this);
            return; //We do not want to go on to the mid part of the action without properly initiating
        }
        currentAction.PerformAction(this);
        #endregion
    }
    void FindCurrentSet()
    {
        if (actionSets == null) return;
        if (actionSets.Count == 1)
        {
            currentSet = (actionSets[0]);
            return;
        }

        List<PlayerActionSet> availableSets = new List<PlayerActionSet>();
        foreach (PlayerActionSet set in actionSets)
        {
            if (set.CheckParameter()) availableSets.Add(set);
        }

        PlayerActionSet highestPrioritySet = null;
        foreach (PlayerActionSet aset in availableSets)
        {
            if (highestPrioritySet == null) highestPrioritySet = aset;
            if (highestPrioritySet.priority < aset.priority) highestPrioritySet = aset;
        }
        currentSet = highestPrioritySet;
    }
    private void HandlePredfinedStateLogic()
    {
        if (currentState == PlayerState.Any || currentState == PlayerState.Idle || currentState == PlayerState.Run)
        {
            if (moveInput.x != 0 && canMove)
            {
                currentState = PlayerState.Run;
            }
            else
            {
                currentState = PlayerState.Idle;
            }
        }
    }

    void Move()
    {
        //Handle Movement

        if (moveInput.x == 0)
        {
            //Not moving
            timeSinceLastMove += Time.deltaTime;
            timeMoving = 0;
        }
        else
        {
            timeSinceLastMove = 0;
            timeMoving += Time.deltaTime;
        }
        if (canMove)
        {
            float accelerationT = Mathf.InverseLerp(0, accelerationTime, timeMoving);
            float accelerationFactor = accelerationCurve.Evaluate(accelerationT);
            //rb.velocity = new Vector3(moveInput.x, 0f) * moveSpeed * accelerationFactor;
            //rb.MovePosition(transform.position + (new Vector3(moveInput.x, 0f) * moveSpeed * accelerationFactor * Time.deltaTime));
            transform.position += new Vector3(moveInput.x, 0f) * moveSpeed * accelerationFactor * Time.deltaTime; //why do I struggle everytime to move things
        }
    }

    void CheckGround()
    {
        Vector3 end = new Vector3(transform.position.x, transform.position.y - groundCheckDistance, transform.position.z);
        //RaycastHit2D groundCast = Physics2D.Linecast(transform.position, end, LayerMask.GetMask("Ground"));
        Collider2D groundCircle = Physics2D.OverlapCircle(end, groundCheckRadius, LayerMask.GetMask("Ground"));

        if (debug)
        {
            Color castColor = (groundCircle != null) ? Color.green : Color.red;
            Debug.DrawLine(transform.position, end, castColor);
            Debug.DrawLine(end, new Vector3(end.x + groundCheckRadius, end.y, end.z));
        }

        OnGround = (groundCircle != null); //Return true when the collider is not null

        if (OnGround)
        {
            timeOnGround += Time.deltaTime;
        }
        else
        {
            timeOnGround = 0;
        }
    }

    public void FlagAnimationEvent(string eventName)
    {
        if (currentAction != null)
        {
            currentAction.FlagAnimationEvent(eventName);
        }
    }

    public void StopMovementUntilOnGround()
    {
        StartCoroutine(StopMoveUntilOnGround());
    }

    IEnumerator StopMoveUntilOnGround()
    {
        //StopAllCoroutines();
        canMove = false;
        while (!OnGround)
        {
            yield return null;
        }
        Debug.Log("Hit Ground");
        canMove = true;
    }
}
