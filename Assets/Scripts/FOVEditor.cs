using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(FieldOfView))]
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.forward, Vector3.right, 360, fow.viewRadius);
        Vector2 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector2 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, (Vector2)fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, (Vector2)fow.transform.position + viewAngleB * fow.viewRadius);
    }
}
