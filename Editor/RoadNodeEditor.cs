using UnityEditor;
using UnityEngine;

namespace lxkvcs.UnityRoadSystem
{
    [CustomEditor(typeof(RoadNode))]
    [CanEditMultipleObjects]
    public class RoadNodeEditor : Editor
    {
        private RoadNode targetNode
        {
            get
            {
                if (targets.Length == 0)
                    return null;
                
                if (targets.Length == 1)
                    return targets[0] as RoadNode;

                if (((RoadNode)targets[0]).SelectionID < ((RoadNode)targets[1]).SelectionID)
                    return targets[0] as RoadNode;

                return targets[1] as RoadNode;
            }
        }

        private RoadNode nextTargetNode
        {
            get
            {
                if (targets.Length < 2)
                    return null;
                
                if (((RoadNode)targets[0]).SelectionID < ((RoadNode)targets[1]).SelectionID)
                    return targets[1] as RoadNode;

                return targets[0] as RoadNode;
            }
        }

        public override void OnInspectorGUI()
        {
            if (targets.Length > 2)
            {
                EditorGUILayout.HelpBox("Unavailable when more than 2 nodes selected", MessageType.Warning);
                return;
            }
            
            if (targets.Length == 1)
                SingleNodeMode();

            else
                MultiNodeMode();
        }


        private void SingleNodeMode()
        {
            NodeInfo(targetNode);
        }


        private void MultiNodeMode()
        {
            NodeInfo(targetNode);
            NodeInfo(nextTargetNode);
            
            EditorGUILayout.Space();

            if (!targetNode.HasConnectionWith(nextTargetNode))
            {
                if (GUILayout.Button("Connect"))
                {
                    targetNode.ConnectNode(nextTargetNode);
                    
                    SceneView.lastActiveSceneView.Repaint();
                }
            }
            else
            {
                GUILayout.Box("These two nodes are connected");
            }
        }


        private void NodeInfo(RoadNode node)
        {
            EditorGUILayout.BeginVertical();
            
            GUILayout.Box(node.ToString());

            string connectionInfo = node.Connections?.Count > 0 ? ":" + Environment.NewLine : "";

            foreach (RoadConnection connection in node.Connections!)
            {
                connectionInfo += Environment.NewLine;
                if (connection.NodeA.ID == node.ID)
                    connectionInfo += connection.NodeB;
                else
                    connectionInfo += connection.NodeA;
            }
            
            EditorGUILayout.HelpBox($"{node.Connections?.Count} connections{connectionInfo}", MessageType.Info);
            
            EditorGUILayout.EndVertical();
        }
    }
}