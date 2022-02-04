using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOnTouch : Interactable
{
    private Collider2D col;
    public bool horizontalFade = true;
    public bool flip;

    public float minFade = 0;
    public float maxFade = 1;

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
        ScreenFader.Instance.ResetFader();
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
        if (TimeOfDayManager.Instance.current != null) minFade = TimeOfDayManager.Instance.current.fadeAmount;
        float a = Mathf.Lerp(minFade, maxFade, t);
        ScreenFader.Instance.SetAlpha(a, 1);
    }
}
