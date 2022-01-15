using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Annoyed : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        TextUnwrapper.Instance.TextEvent += AnnoyedAF;
    }

    void AnnoyedAF(string id)
    {
        if (id == "Annoyed")
        {
            Debug.LogError("Annoying dude");
        }
    }
}
