using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using System.Linq;

public class CameraZoom : MonoBehaviour
{
    public static CameraZoom Main;
    public float originalZoom;
    Tween zoomTween;
    Camera cam;
    CinemachineBrain brain;
    CinemachineVirtualCamera vcam;
    
    public List<CameraRequest<float>> requests = new List<CameraRequest<float>>();
    public CameraRequest<float> currentRequest;

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

    private void LateUpdate() {
        if (requests.Count == 0) return;
        Debug.Log($"Request Count: {requests.Count}");
        if(requests.Where(r => r.reset == false).Any())
        {
            CameraRequest<float> request = requests.Where(r => r.reset == false).First();
            SetZoom(request);
        }
        else
        {
            CameraRequest<float> request = requests.Where(r => r.reset == true).First();
            SetZoom(request);
        }

        requests.Clear();
    }

    private void SetZoom(CameraRequest<float> request)
    {
        currentRequest = request;
        zoomTween?.Kill();
        zoomTween = DOTween.To(() => vcam.m_Lens.OrthographicSize, x => vcam.m_Lens.OrthographicSize = x, request.to, request.speed).SetEase(Ease.InOutCubic);
    }

    private void Start() {
        cam = Camera.main;
        brain = cam.GetComponent<CinemachineBrain>();
        originalZoom = (brain.ActiveVirtualCamera as CinemachineVirtualCamera).m_Lens.OrthographicSize;
        vcam = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
    }

    public void ZoomCamera(float zoomAmount, float zoomTime)
    {
        requests.Add(new CameraRequest<float>(zoomAmount, zoomTime, false));
    }

    public void ResetZoom(float zoomTime = 1f)
    {
        requests.Add(new CameraRequest<float>(originalZoom, zoomTime, true));
    }
}
