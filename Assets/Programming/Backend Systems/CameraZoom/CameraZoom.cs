using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class CameraZoom : MonoBehaviour
{
    public static CameraZoom Main;

    public float originalZoom;
    Tween zoomTween;
    Camera cam;
    CinemachineBrain brain;
    CinemachineVirtualCamera vcam;

    private void Awake()
    {
        if (Main == null)
        {
            Main = this;
        }
        else 
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        cam = Camera.main;
        brain = cam.GetComponent<CinemachineBrain>();
        originalZoom = (brain.ActiveVirtualCamera as CinemachineVirtualCamera).m_Lens.OrthographicSize;
        vcam = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
    }

    public void ZoomCamera(float zoomAmount, float zoomTime)
    {
        zoomTween?.Kill();
        zoomTween = DOTween.To(() => vcam.m_Lens.OrthographicSize, x => vcam.m_Lens.OrthographicSize = x, zoomAmount, zoomTime).SetEase(Ease.InOutBack);
    }

    public void ResetZoom(float zoomTime = 1f)
    {
        zoomTween?.Kill();
        zoomTween = DOTween.To(() => vcam.m_Lens.OrthographicSize, x => vcam.m_Lens.OrthographicSize = x, originalZoom, zoomTime).SetEase(Ease.InOutBack);
    }
}
