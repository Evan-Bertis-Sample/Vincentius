using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightedLevelDoor : LevelDoorway
{
    public Sprite[] highlightAnimation;
    public float animationLength = 0.5f;
    private float animationTime;

    private SpriteRenderer sr;

    public override void OnStart()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public override void OnDoorwayEnter()
    {
        base.OnDoorwayEnter();
        animationTime = 0;
    }

    public override void OnDoorwayHover()
    {
        base.OnDoorwayHover();
        animationTime += Time.deltaTime;

        if (animationTime >= animationLength)
        {
            animationTime = 0;
        }

        int spriteIndex = Mathf.RoundToInt(Mathf.Lerp(0, highlightAnimation.Length - 1, Mathf.InverseLerp(0, animationLength, animationTime)));

        sr.sprite = highlightAnimation[spriteIndex];
    }

    public override void OnDoorwayLeave()
    {
        base.OnDoorwayLeave();
        sr.sprite = null;
    }
}
