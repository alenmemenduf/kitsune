using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[RequireComponent (typeof (Player))]
public class PlayerHingeEditor : Editor
{
    // Start is called before the first frame update

    private void OnSceneGUI()
    {
        Player player = (Player)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(player.transform.position, Vector3.forward, Vector3.right, 360, player.hingeViewRadius);
    }
}
