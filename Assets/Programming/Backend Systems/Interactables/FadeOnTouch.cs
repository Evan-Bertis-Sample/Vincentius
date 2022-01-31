using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOnTouch : Interactable
{
    private Collider2D col;
    public bool horizontalFade = true;
    public bool flip;

    private float min;
    private float max;
    private float current;

    private void Start()
    {
        col = GetComponent<Collider2D>();    
    }

    public override void OnContact(GameObject player)
    {
        if (horizontalFade)
        {
            min = col.bounds.min.x;
            max = col.bounds.max.x;
        }
        else
        {
            min = col.bounds.min.y;
            max = col.bounds.max.y;
        }
    }

    public override void OnExit(GameObject player)
    {
        ScreenFader.Instance.SetAlpha(0, 1);
    }

    public override void OnHold(GameObject player)
    {
        if (horizontalFade)
        {
            current = player.transform.position.x;
        }
        else
        {
            current = player.transform.position.y;
        }

        float t = Mathf.InverseLerp(min, max, current);
        if (flip) t = 1 - t;
        ScreenFader.Instance.SetAlpha(t, 1);
    }
}
