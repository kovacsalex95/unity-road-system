using System;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace lxkvcs.UnityRoadSystem
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

            Vector3[] newPositions = new Vector3[system.Nodes.Count];

            EditorGUI.BeginChangeCheck();

            for (int i = 0; i < system.Nodes.Count; i++)
                newPositions[i] = Handles.PositionHandle(system.Nodes[i].WorldPosition, transform.rotation);

            if (!EditorGUI.EndChangeCheck()) return;

            for (int i = 0; i < newPositions.Length; i++)
            {
                Vector3 newLocalPosition = system.SnapPointToGrid(newPositions[i]);
                system.Nodes[i].MoveTo(newLocalPosition);
            }
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
