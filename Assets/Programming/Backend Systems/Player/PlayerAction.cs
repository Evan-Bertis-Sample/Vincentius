using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerAction : ScriptableObject
{
    #region Properties
    public InputAction binding;
    public PlayerState newState;
    public List<PlayerState> canBeStates = new List<PlayerState>()
    {
        PlayerState.Any
    };
    public bool checkParameter;
    public bool checkBinding;
    public bool checkBindingStart;
    public bool checkParameterAfterInit;
    public bool checkBindingAfterInit;
    public bool checkBindingStartAfterInit;
    public bool bindingTriggeredThisFrame;
    public bool interruptable = false;

    public float timeElapsed { get; private set; }
    public bool performingAction { get; private set; }
    public bool waitForAnimationEvent;
    public bool animationFlag;
    public string animationEventName;

    #endregion

    #region Abstract Functions
    public abstract bool CheckParameter(PlayerController controller); //Parameter that dictates whether or not the actions can be performed - touching ground, touching wall, etc.
    public abstract bool CheckForOverrideExit(PlayerController controller); //If a certain thing is true, the action is exited immediately
    protected abstract void Initiate(PlayerController controller); //A function that is called once when the actions starts
    protected abstract void OnHold(PlayerController controller); //A function that is called multiple times until the action ends
    protected abstract void OnExit(PlayerController controller, bool interrupted); //A function that is called once the action ends
    #endregion

    #region Public Functions

    public void Init()
    {
        binding.Enable();
        performingAction = false;
        timeElapsed = 0f;
        if (checkBinding || checkBindingAfterInit || checkBindingStart)
        {
            binding.performed += context => FlagBindingTrigger(context, true);
        }
        Start();
    }

    public virtual void Start()
    {
        //Debug.Log(name);
        //You can change it
    }

    public bool CheckAction(PlayerController controller) //Checks whether or not the action should be performed using default values
    {
        return (CheckAction(controller, checkParameter, checkBinding, checkBindingStart));
    }

    public bool CheckActionAfterInit(PlayerController controller)
    {
        return (CheckAction(controller, checkParameterAfterInit, checkBindingAfterInit, checkBindingStartAfterInit) && !CheckForOverrideExit(controller));
    }

    public void InitiateAction(PlayerController controller) //Starts the action
    {
        controller.currentState = newState;
        if (waitForAnimationEvent && !animationFlag) return;
        Initiate(controller);
        performingAction = true;
        timeElapsed = 0f;

        if (controller.debug)
        {
            Debug.Log(name.ToUpper() + " has been Initated");
        }
    }

    public void PerformAction(PlayerController controller) //Performs the action
    {
        OnHold(controller);
        timeElapsed += Time.deltaTime;

        if (controller.debug)
        {
            Debug.Log(name.ToUpper() + " is being Performed");
        }
    }

    public bool CheckForExit(PlayerController controller)
    {
        if (!performingAction) return true;
        return (!CheckActionAfterInit(controller) || CheckForOverrideExit(controller));
    }

    public void ExitAction(PlayerController controller, bool interrupted) //Ends the action
    {
        OnExit(controller, interrupted);
        //Reset the flags
        performingAction = false;
        animationFlag = false;
        controller.currentState = PlayerState.Idle;
        timeElapsed = 0f;

        if (controller.debug)
        {
            Debug.Log(name.ToUpper() + " has been Exited");
        }
    }

    public void FlagAnimationEvent(string eventName)
    {
        if (eventName == animationEventName) animationFlag = true;
    }

    #endregion

    #region Internal Functions
    private bool CheckStates(PlayerState currentState)
    {
        if (canBeStates.Contains(PlayerState.Any) || currentState == newState) return true;
        return (canBeStates.Contains(currentState));
    }

    private bool CheckBinding(bool cbs)
    {
        bool bindingTriggered = bindingTriggeredThisFrame;
        bindingTriggeredThisFrame = false;
        if (cbs)
        {
            return (binding.ReadValue<float>() > 0 && bindingTriggered);
        }
        else return (binding.ReadValue<float>() > 0);
    }

    private void FlagBindingTrigger(InputAction.CallbackContext context, bool flag)
    {
        //Debug.Log(name.ToUpper() + " binding Init at " + context.startTime);
        bindingTriggeredThisFrame = flag;
    }

    private bool CheckAction(PlayerController controller, bool checkParameter, bool checkBinding, bool checkBindingStart) //Checks whether or not the action should be performed using different Values
    {
        //Debug.Log(this.name.ToUpper() + " has been checked");

        //Guard clause - if it is set that neither the parameter or binding should be checked, then return if the player is in the right state
        if (!checkParameter && !checkBinding)
        {
            //bindingTriggeredThisFrame = false; //Update Value
            return (CheckStates(controller.currentState));
        }
        //Guard clause - if the player is in the wrong state, then return.
        if (!CheckStates(controller.currentState))
        {
            //bindingTriggeredThisFrame = false; //Update Value
            return false;
        }

        //Debug.Log(this.name.ToUpper() + " is in the right state to perform");

        if (checkParameter)
        {
            if (checkBinding) return (CheckParameter(controller) && CheckBinding(checkBindingStart));

            //bindingTriggeredThisFrame = false;
            return (CheckParameter(controller));
        }
        else return (CheckBinding(checkBindingStart));
    }

    #endregion
}
