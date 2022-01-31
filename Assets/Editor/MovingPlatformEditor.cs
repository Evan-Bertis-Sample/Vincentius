using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MovingPlatform))]
public class MovingPlatformEditor : Editor
{
    private void OnSceneGUI() {
        MovingPlatform mp = (MovingPlatform)target;

        if (mp.travelPoints == null) return;
        if (mp.travelPoints.Count == 0) return;

        for(int i = 0; i < mp.travelPoints.Count; i++)
        {
            mp.travelPoints[i] = DrawHandles(mp, mp.travelPoints[i]);
        }

        for(int i = 0; i < mp.travelPoints.Count - 1; i++)
        {
            Vector3 current = mp.travelPoints[i];
            Vector3 next = mp.travelPoints[i + 1];
            Handles.DrawLine(current, next);
        }

        if (mp.returnToStart)
        {
            Handles.DrawLine(mp.travelPoints[0], mp.travelPoints[mp.travelPoints.Count - 1]);
        }
    }

    private Vector3 DrawHandles(MovingPlatform mp, Vector3 bound)
    {
        EditorGUI.BeginChangeCheck();
        Vector3 newTargetPosition = Handles.FreeMoveHandle(bound, Quaternion.identity, 1f, Vector3.one * 0.5f, Handles.SphereHandleCap);
        bound = newTargetPosition;
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(mp, "Change Look At Target Position");
        }

        return newTargetPosition;
    }
}
