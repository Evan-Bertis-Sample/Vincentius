using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAppearPlatform : MonoBehaviour
{
    public List<PlayerAction> triggerActions = new List<PlayerAction>();

    public bool platformActive;
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    public List<Collider2D> cols = new List<Collider2D>();
    private SpriteRenderer sr;
    
    private void Start()
    {
        PlayerController.OnPlayerAction += action => HandlePlatformState(action);
        LevelManager.OnSceneChange += sceneName => 
        {
            PlayerController.OnPlayerAction -= action => HandlePlatformState(action);
            Debug.Log("Unsubscribed");
        };
 
        sr = GetComponent<SpriteRenderer>();

        if (cols.Count == 0) cols.Add(GetComponent<Collider2D>());
        UpdatePlatform();
    }

    public void HandlePlatformState(PlayerAction action)
    {
        if (!triggerActions.Contains(action)) return; //We do not trigger this action
        UpdatePlatform();
    }

    private void UpdatePlatform()
    {
        if (sr == null || cols.Contains(null)) return;
        if (platformActive)
        {
            platformActive = false;
            SetColliderState(false);
            sr.sprite = inactiveSprite;
        }
        else
        {
            platformActive = true;
            SetColliderState(true);
            sr.sprite = activeSprite;
        }
    }


    private void SetColliderState(bool active)
    {
        if (cols == null) return;

        foreach (Collider2D col in cols)
        {
            if (col == null) continue;
            col.enabled = active;
        }
    }
}
