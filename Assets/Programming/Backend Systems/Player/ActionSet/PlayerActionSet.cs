using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Action Set/No Parameter")]
public class PlayerActionSet : ScriptableObject
{
    public int priority;
    public List<PlayerAction> actions;

    public void Init()
    {
        if (actions == null) return;
        foreach (PlayerAction act in actions)
        {
            act.Init();
        }
    }
    public virtual bool CheckParameter()
    {
        return true;
    }
}
