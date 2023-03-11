using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using Resources = unity_road_system.Resources;


namespace unity_road_system.Editor
{
    [EditorTool("Road nodes", typeof(RoadSystem))]
    public class RoadSystemEditorTool : EditorTool
    {
        private Transform transform => Selection.transforms.Length == 1 ? Selection.transforms[0] : null;

        private RoadSystem system =>
            Selection.transforms.Length == 1 ? Selection.transforms[0].GetComponent<RoadSystem>() : null;

        private GUIContent _toolbarIcon;

        public override GUIContent toolbarIcon => _toolbarIcon;


        public void OnEnable()
        {
            _toolbarIcon = Resources.ToolbarIcon;
        }
        
        
        public override void OnToolGUI(EditorWindow window)
        {
            if (system == null) return;
            if (system.Nodes.Count == 0) return;

            Dictionary<uint, Vector3> newPositions = new Dictionary<uint, Vector3>();

            EditorGUI.BeginChangeCheck();

            foreach (KeyValuePair<uint, RoadNode> node in system.Nodes)
                newPositions.Add(node.Key, Handles.PositionHandle(node.Value.WorldPosition, transform.rotation));

            if (!EditorGUI.EndChangeCheck()) return;

            foreach(KeyValuePair<uint, Vector3> position in newPositions)
                system.Nodes[position.Key].MoveTo(position.Value);
        }
        
        
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        
        private static void OnSelectionChanged()
        {
            try
            {
                RoadSystem selectedSystem = Selection.activeGameObject?.GetComponent<RoadSystem>();
                if (selectedSystem)
                {
                    selectedSystem.ReloadNodes();
                    ToolManager.SetActiveTool<RoadSystemEditorTool>(); //.OnToolGUI(EditorWindow.focusedWindow);
                }
            }
            
            catch (Exception e) { /* ... */ }
        }
    }
}
