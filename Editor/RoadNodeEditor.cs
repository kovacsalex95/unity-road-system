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
            
            GUILayout.Box("Connections:");
            if (((RoadNode) target).HasAnyConnection && GUILayout.Button("Clear all"))
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

            bool twoConnected = targets.Length == 2 && (((RoadNode)targets[0]).HasConnectionWith((RoadNode)targets[1]) || ((RoadNode)targets[1]).HasConnectionWith((RoadNode)targets[0]));

            EditorGUILayout.Space();

            RoadSystem system = ((RoadNode)target).ParentSystem;

            GUILayout.Box("Connections:");
            
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Connect"))
                system.ConnectSelectedNodes();

            if (twoConnected && GUILayout.Button("Disconnect"))
            {
                ((RoadNode)targets[0]).DisconnectNode(targets[1] as RoadNode);
                ((RoadNode)targets[1]).DisconnectNode(targets[0] as RoadNode);
            }
                

            if (hasConnection && GUILayout.Button("Clear all"))
                system.DisconnectSelectedNodes();
            
            GUILayout.EndHorizontal();
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