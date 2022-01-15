using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(BackgroundObjectSpawner))]
public class BackgroundObjectSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    { 
        base.OnInspectorGUI();
        BackgroundObjectSpawner pt = (BackgroundObjectSpawner)target;

        if (GUILayout.Button("Populate Objects"))
        {
            pt.GenerateSpawnableObjects();
            pt.SpawnObjects();
        }

        if (GUILayout.Button("Clear Objects"))
        {
            pt.ClearObjects();
        }
    }


    public void OnSceneGUI()
    {
        BackgroundObjectSpawner pt = (BackgroundObjectSpawner)target;

        Handles.color = Color.white;
        Handles.DrawLine(pt.leftBound, pt.rightBound);

        DrawCurve(pt);
        Handles.color = pt.handleColor;
        DrawHandles(pt, ref pt.leftBound);
        DrawHandles(pt, ref pt.rightBound);
        FixHandles(pt);
    }

    private void DrawCurve(BackgroundObjectSpawner pt)
    {
        float distance = pt.rightBound.x - pt.leftBound.x;
        int numPoints = Mathf.RoundToInt(distance * 2f);
        
        Vector3[] points = new Vector3[numPoints];

        for (int i = 0; i < numPoints; i++)
        {
            float t = Mathf.InverseLerp(0, numPoints, i);
            float y = pt.spawnCurve.Evaluate(t);
            float x = Mathf.Lerp(pt.leftBound.x, pt.rightBound.x, t);

            points[i] = new Vector3(x, pt.transform.position.y + y, 0);
        }

        for (int i = 0; i < points.Length - 1; i++) 
        {
            Handles.color = Color.blue;
            Handles.DrawLine(points[i], points[i + 1]);
        }        
    }

    private void DrawHandles(BackgroundObjectSpawner pt, ref Vector3 bound)
    {
        EditorGUI.BeginChangeCheck();
        Vector3 newTargetPosition = Handles.FreeMoveHandle(bound, Quaternion.identity, pt.handleSize, Vector3.one * 0.5f, Handles.SphereHandleCap);
        newTargetPosition = new Vector3(newTargetPosition.x, pt.transform.position.y, newTargetPosition.z);
        bound = newTargetPosition;
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(pt, "Change Look At Target Position");
        }
    }

    private void FixHandles(BackgroundObjectSpawner pt)
    {
        if(pt.rightBound.x < pt.leftBound.x)
        {
            Vector3 temp = pt.rightBound;
            pt.rightBound = pt.leftBound;
            pt.leftBound = temp;
        }
    }
}
