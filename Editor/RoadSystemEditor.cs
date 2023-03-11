using System;
using UnityEditor;
using UnityEngine;


namespace lxkvcs.UnityRoadSystem
{
    [CustomEditor(typeof(RoadSystem))]
    public class RoadSystemEditor : Editor
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
            
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();

            if (GUILayout.Button($"Snap to grid: {system.enableSnapping}", GUILayout.Width(inspectorWidth / 3)))
                system.enableSnapping = !system.enableSnapping;

            system.snappingDistance = EditorGUILayout.FloatField(system.snappingDistance);
            
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Create node", GUILayout.Height(40)))
                system.CreateNode();
        }
        
        
        private void OnTargetUpdate(object sender, EventArgs e)
        {
            EditorUtility.SetDirty(system);
        }
    }
}
