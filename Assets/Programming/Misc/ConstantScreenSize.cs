using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ConstantScreenSize : MonoBehaviour
{
    private float startingOrthoSize;
    private Vector3 startingSize;
    private float ratio;

    private float curOrthoSize;

    CinemachineVirtualCamera vcam;

    void Start()
    {
        startingSize = transform.localScale;
        vcam = ((Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera) as CinemachineVirtualCamera);
        startingOrthoSize = vcam.m_Lens.OrthographicSize;

        ratio = startingSize.magnitude / startingOrthoSize;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = startingSize.normalized * vcam.m_Lens.OrthographicSize * ratio;
    }
}
