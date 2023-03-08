using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common;
using lxkvcs.UnityRoadSystem;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.Rendering;
using UnityEngine;

[EditorTool("Road nodes", typeof(RoadSystem))]
public class RoadSystemEditorTool : EditorTool
{
    private Transform transform => Selection.transforms.Length == 1 ? Selection.transforms[0] : null;
    private RoadSystem system => Selection.transforms.Length == 1 ? Selection.transforms[0].GetComponent<RoadSystem>() : null;
    
    
    public override void OnToolGUI(EditorWindow window)
    {
        if (system == null) return;
        if (system.Nodes.Count == 0) return;

        Vector3[] newPositions = new Vector3[system.Nodes.Count];

        EditorGUI.BeginChangeCheck();

        for (int i = 0; i < system.Nodes.Count; i++)
            newPositions[i] = Handles.PositionHandle(system.transform.TransformPoint(system.Nodes[i].Position), transform.rotation);
        
        if (!EditorGUI.EndChangeCheck()) return;

        for (int i = 0; i < newPositions.Length; i++)
        {
            Vector3 newLocalPosition = system.transform.InverseTransformPoint(system.SnapPointToGrid(newPositions[i]));
            system.Nodes[i].MoveTo(newLocalPosition);
        }
    }
}
