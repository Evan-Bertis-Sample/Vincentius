using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Vector3 startingPosition;
    private float startingZ;

    Camera cam;
    public Vector3 startingCamPosition;

    public float xShiftFactor = 1f;
    public float yShiftFactor = 1f;
    public float relativeDistance;
    
    void Start()
    {
        startingPosition = transform.position;
        startingZ = startingPosition.z;
        cam = Camera.main;
        startingCamPosition = cam.transform.position;

        relativeDistance = InvLerp(0, 5, startingZ);

        LevelManager.OnSceneLateChange += name => startingCamPosition = cam.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 camShift = cam.transform.position - startingCamPosition;
        transform.position = startingPosition +
            new Vector3(camShift.x * relativeDistance * xShiftFactor,
            camShift.y * relativeDistance * yShiftFactor,
            0);
    }

    float InvLerp(float a, float b, float v)
    {
        return (v - a) / (b - a);
    }
}
