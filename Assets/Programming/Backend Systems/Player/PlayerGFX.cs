using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGFX : MonoBehaviour
{
    [System.Serializable]
    public class OverrideSprite{
        public Sprite newSprite;
        public int priority;
        public bool flip;
        public OverrideSprite(Sprite newSprite, int priority, bool flip){
            this.newSprite = newSprite;
            this.priority = priority;
            this.flip = flip;
        }
    }

    PlayerController controller;
    public SpriteRenderer sr;
    public bool lockSpriteDir = false;
    PlayerState lastAnimatedState;

    public Sprite[] fallingSprites;
    public float maxFallVelocity = 2f;

    public List<OverrideSprite> overrides = new List<OverrideSprite>();

    Animator animator;
    void Start()
    {
        controller = GetComponent<PlayerController>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update() {
        HandleFallingAnimation();
    }

    void LateUpdate() {
        if (overrides.Count <= 0)
        {
            HandleSpriteDirection();
            HandleAnimator();
        }
        else
        {
            HandleOverrides();
        }
    }

    void HandleSpriteDirection()
    {
        if (!lockSpriteDir)
        {
            if (controller.moveInput.x > 0) sr.flipX = false;
            if (controller.moveInput.x < 0) sr.flipX = true;
            //When the player is moving, leave the direction as is
        }
    }
    
    void HandleAnimator()
    {
        animator.SetInteger("State", (int)controller.currentState);
    }

    private void HandleFallingAnimation()
    {
        //You are not falling if you are on the ground, and if you are not idling or walking
        if (controller.OnGround || !(controller.currentState == PlayerState.Idle || controller.currentState == PlayerState.Run) ) return;

        float velY = controller.rb.velocity.y;

        int spriteIndex = (int)Map(0, -maxFallVelocity, 0, fallingSprites.Length - 1, velY);
        RequestOverride(fallingSprites[spriteIndex], 0);
    }

    void HandleOverrides(){
        if (overrides == null) return;
        if (overrides.Count == 0) return;

        if (overrides.Count == 1)
        {
            //No need to loop through requests
            PerformOverride(overrides[0]);
            return;
        }

        int largestPriority = int.MinValue;
        OverrideSprite selectedOS = null;
        foreach(OverrideSprite os in overrides){
            if (os.priority > largestPriority){
                selectedOS = os;
                largestPriority = os.priority;
            }
        }

        PerformOverride(selectedOS);
    }

    public void PerformOverride(OverrideSprite os) 
    {
        if (os.newSprite != null)
        {
            animator.SetInteger("State", 0);
            sr.sprite = os.newSprite;
        }
        else
        {
            //Only the flip was needed, we still need the animation
            HandleAnimator();
        }
        sr.flipX = os.flip;

        overrides.Clear();
    }

    public void RequestOverride(Sprite newSprite, int priority, bool flip)
    {
        overrides.Add(new OverrideSprite(newSprite, priority, flip));
    }

    public void RequestOverride(Sprite newSprite, int priority)
    {
        overrides.Add(new OverrideSprite(newSprite, priority, sr.flipX));
    }

    public void RequestOverride(int priority, bool flip)
    {
        overrides.Add(new OverrideSprite(null, priority, flip));
    }

    float Map(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {
        float t = Mathf.InverseLerp(OldMin, OldMax, OldValue);
        return Mathf.Lerp(NewMin, NewMax, t);
    }
}
