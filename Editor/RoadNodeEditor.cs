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

                if (((RoadNode)targets[0]).ID < ((RoadNode)targets[1]).ID)
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

            if (GUILayout.Button("Connect"))
            {
                targetNode.ConnectNode(nextTargetNode);
                Selection.activeGameObject = targetNode.ParentSystem.gameObject;
            }
        }


        private void NodeInfo(RoadNode node)
        {
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.HelpBox($"Road node #{node.ID}" + System.Environment.NewLine + $"Connections: {node.Connections?.Count}", MessageType.Info);
            
            EditorGUILayout.EndVertical();
        }
    }
}