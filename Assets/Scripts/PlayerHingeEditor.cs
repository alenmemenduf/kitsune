using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor (typeof (Player))]
public class PlayerHingeEditor : Editor
{

    private void OnSceneGUI()
    {
        Player player = (Player)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(player.transform.position, Vector3.forward, Vector3.right, 360, player.hingeViewRadius);
    }
}
