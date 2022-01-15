using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForce : MonoBehaviour
{
    // Start is called before the first frame update
    public ForceMode2D mode;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * 2f, mode);
    }
}
