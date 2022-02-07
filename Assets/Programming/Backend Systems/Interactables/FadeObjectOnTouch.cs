using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FadeObjectOnTouch : Interactable
{
    public SpriteRenderer sr;
    public float fadeTime = 1f;

    void Start()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
    }


    public override void OnContact(GameObject player)
    {
        sr.DOFade(0, fadeTime);
    }

    public override void OnExit(GameObject player)
    {

    }

    public override void OnHold(GameObject player)
    {

    }
}
