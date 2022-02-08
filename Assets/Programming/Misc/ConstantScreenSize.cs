using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ConstantScreenSize : MonoBehaviour
{
    [SerializeField]private float startingOrthoSize;
    [SerializeField]private Vector3 startingSize;
    [SerializeField]private float ratio;

    [SerializeField]private float curOrthoSize;

    Camera mCam;

    void Start()
    {
        startingSize = transform.localScale;
        mCam = Camera.main;
        startingOrthoSize = mCam.orthographicSize;

        ratio = startingSize.magnitude / startingOrthoSize;
    }

    // Update is called once per frame
    void Update()
    {
        curOrthoSize = mCam.orthographicSize;
        transform.localScale = startingSize.normalized * curOrthoSize * ratio;
    }
}
