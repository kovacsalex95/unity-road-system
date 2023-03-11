using System;
using UnityEditor;
using UnityEngine;


namespace unity_road_system.Editor
{
    [CustomEditor(typeof(RoadSystem))]
    public class RoadSystemEditor : UnityEditor.Editor
    {
        private RoadSystem system => target as RoadSystem;

        
        private void OnEnable()
        {
            system.OnSystemUpdate += OnTargetUpdate;
        }

        
        private void OnDisable()
        {
            system.OnSystemUpdate -= OnTargetUpdate;
        }
        

        public override void OnInspectorGUI()
        {
            float inspectorWidth = EditorGUIUtility.currentViewWidth;

            if (GUILayout.Button("Create node", GUILayout.Height(40)))
                system.CreateNode();
            
            if (GUILayout.Button("Clear nodes"))
                system.ReloadNodes();
        }
        
        
        private void OnTargetUpdate(object sender, EventArgs e)
        {
            Debug.Log("Repaint");
            SceneView.lastActiveSceneView.Repaint();
            EditorUtility.SetDirty(system);
        }
    }
}
