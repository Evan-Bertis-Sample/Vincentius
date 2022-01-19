using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyExtraPlayers : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (LevelManager.Instance.player.gameObject != gameObject)
        {
            Destroy(gameObject);
        }
    }
}
