using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomOnTouch : Interactable
{
    public float zoomAmount = 4f;
    public float zoomSpeed = 1f;

    public override void OnContact()
    {
        CameraZoom.Main.ZoomCamera(zoomAmount, zoomSpeed);
    }

    public override void OnExit()
    {
        CameraZoom.Main.ResetZoom(zoomSpeed);
    }

    public override void OnHold()
    {

    }
}
