using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class WorldBoundary : MonoBehaviour
{
    private Vector3 originalPos;
    void Start()
    {
        PolygonCollider2D boundary = GetComponent<PolygonCollider2D>();
        if (boundary == null) return;
        boundary.isTrigger = true;
        LevelManager.Instance.SetLevelBoundary(boundary);
    }

    private void Update() {
        transform.position = originalPos;
    }

    private void SetBoundary(Collider2D boundary)
    {
        boundary.isTrigger = true;
        ICinemachineCamera activeCamera = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera;
        GameObject camObj = Camera.main.gameObject;

        if (camObj.transform.childCount == 0)
        {
            transform.parent = camObj.transform;
        }
        else
        {
            DestroyImmediate(camObj.transform.GetChild(0).gameObject);
            transform.parent = camObj.transform;
            Debug.Log("Set New Boundary");
        }
        activeCamera.VirtualCameraGameObject.GetComponent<CinemachineConfiner>().m_BoundingShape2D = boundary;
    }
}
