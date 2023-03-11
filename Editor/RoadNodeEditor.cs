using System;
using UnityEditor;
using UnityEngine;


namespace unity_road_system.Editor
{
    [CustomEditor(typeof(RoadNode))]
    [CanEditMultipleObjects]
    public class RoadNodeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (targets.Length == 1)
                SingleNodeMode();

            else
                MultiNodeMode();
        }


        private void SingleNodeMode()
        {
            NodeInfo(target as RoadNode);

            EditorGUILayout.Space();
            
            if (((RoadNode) target).HasAnyConnection && GUILayout.Button("Disconnect"))
                ((RoadNode)target).ParentSystem.DisconnectSelectedNodes();
        }


        private void MultiNodeMode()
        {
            bool hasConnection = false;
            
            foreach (object node in targets)
            {
                NodeInfo(node as RoadNode);
                
                if (!hasConnection && ((RoadNode)node).HasAnyConnection)
                    hasConnection = true;
            }

            EditorGUILayout.Space();

            RoadSystem system = ((RoadNode)target).ParentSystem;

            if (GUILayout.Button("Connect"))
                system.ConnectSelectedNodes();

            if (hasConnection && GUILayout.Button("Disconnect"))
                system.DisconnectSelectedNodes();
        }


        private void NodeInfo(RoadNode node)
        {
            EditorGUILayout.BeginVertical();
            
            GUILayout.Box(node.ToString());

            string connectionInfo = node.Connections?.Count > 0 ? ":" + Environment.NewLine : "";

            foreach (uint nodeID in node.ConnectedNodeIDs)
            {
                connectionInfo += Environment.NewLine;
                connectionInfo += node.ParentSystem.Nodes[nodeID];
            }
            
            EditorGUILayout.HelpBox($"{node.Connections?.Count} connections{connectionInfo}", MessageType.Info);
            
            EditorGUILayout.EndVertical();
        }
    }
}