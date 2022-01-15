using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Vector2 offset = Vector2.zero;
    public float zPos = 0;
    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        transform.position = mainCamera.transform.position + (Vector3)offset;
        transform.position = new Vector3(transform.position.x, transform.position.y, zPos);
        transform.parent = mainCamera.transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
