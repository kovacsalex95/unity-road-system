// BUG: Snap to grid after moving individual RoadNodes
// BUG: Road connections lost after some actions
// BUG: Undo not working properly
// TODO: Create nodes from the scene view


using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace lxkvcs.UnityRoadSystem
{
    [ExecuteInEditMode]
    public class RoadSystem : MonoBehaviour
    {
        const float NEW_NODE_DISTANCE = 2f; // in units (m)
        
        
        [SerializeField]
        public bool enableSnapping = true;
        
        [SerializeField]
        public float snappingDistance = 0.25f;

        private List<RoadNode> nodes = null;
        private Dictionary<int, int> selectedNodeIDs = new Dictionary<int, int>();

        private bool roadPartSelected = false;

        public event EventHandler OnSystemUpdate = null; 

        
        public List<RoadNode> Nodes
        {
            get
            {
                if (nodes == null)
                {
                    nodes = new List<RoadNode>();
                    
                    RoadNode[] childNodes = GetComponentsInChildren<RoadNode>();
                    nodes.AddRange(childNodes);

                    foreach (RoadNode node in nodes)
                    {
                        node.Init(this as RoadSystem);
                    }
                    
                    UpdateSystem();
                }

                return nodes;
            }
        }
        
        
        public void ReloadNodes()
        {
            nodes.Clear();
            nodes = null;
        }


        public void CreateNode()
        {
            Vector3 position = Vector3.zero;

            if (Nodes.Count > 0)
            {
                position = nodes[nodes.Count - 1].Position;
                float randomDegree = Random.Range(0f, 360f);
                float randomX = Mathf.Sin(randomDegree * Mathf.Deg2Rad);
                float randomY = Mathf.Cos(randomDegree * Mathf.Deg2Rad);
                position += new Vector3(randomX * NEW_NODE_DISTANCE, 0, randomY * NEW_NODE_DISTANCE);
            }

            int nodeID = 0;
            foreach (RoadNode _node in Nodes)
            {
                if (_node.ID > nodeID)
                    nodeID = _node.ID;
            }
            nodeID++;

            GameObject nodeObject = new GameObject();
            nodeObject.transform.parent = transform;
            nodeObject.transform.localEulerAngles = Vector3.zero;
            nodeObject.transform.localScale = Vector3.one;
            nodeObject.name = $"Node #{nodeID}";

            RoadNode node = nodeObject.AddComponent<RoadNode>();
            node.Init(this as RoadSystem);
            node.MoveTo(SnapPointToGrid(position));
            node.ID = nodeID;

            var meshFilter = nodeObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = Resources.RoadNodeMesh;
            var meshRenderer = nodeObject.AddComponent<MeshRenderer>();
            meshRenderer.receiveShadows = false;
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.sharedMaterial = new Material(Resources.RoadNodeMaterial);

            Nodes.Add(node);

            UpdateSystem();
        }


        public void UpdateSystem()
        {
            // TODO: Updates etc
            
            if (OnSystemUpdate != null)
                OnSystemUpdate.Invoke(this as RoadSystem, new EventArgs());
        }
        

        public static RoadSystem FirstSystem()
        {
            RoadSystem[] objects = FindObjectsOfType<RoadSystem>();
            if (objects.Length == 0)
                return null;

            return objects[0];
        }
        
        
        public Vector3 SnapPointToGrid(Vector3 point)
        {
            if (!enableSnapping || snappingDistance <= 0.001f)
                return point;

            float x = Mathf.Round(point.x / snappingDistance) * snappingDistance;
            float y = Mathf.Round(point.y / snappingDistance) * snappingDistance;
            float z = Mathf.Round(point.z / snappingDistance) * snappingDistance;

            return new Vector3(x, y, z);
        }
        
        
        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        
        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        
        private void OnSelectionChanged()
        {
            List<int> checkedInstanceIDs = new List<int>();

            foreach (var obj in Selection.objects)
            {
                if (!obj is GameObject)
                    continue;

                if (((GameObject)obj).GetComponent<RoadNode>() == null)
                {
                    if (((GameObject)obj).GetComponent<RoadSystem>() != null)
                        roadPartSelected = true;
                    
                    continue;
                }

                int instanceID = ((GameObject)obj).GetInstanceID();
                checkedInstanceIDs.Add(instanceID);

                if (!selectedNodeIDs.ContainsKey(instanceID))
                {
                    selectedNodeIDs.Add(instanceID, ((GameObject)obj).GetComponent<RoadNode>().ID);
                    ((GameObject)obj).GetComponent<RoadNode>().Select();
                }
            }

            List<int> selectionToDelete = new List<int>();
            foreach (KeyValuePair<int, int> ids in selectedNodeIDs)
            {
                if (checkedInstanceIDs.Contains(ids.Key))
                    continue;

                foreach (RoadNode node in Nodes)
                {
                    if (node.ID != ids.Value)
                        continue;
                    
                    node.Deselect();
                }
                
                selectionToDelete.Add(ids.Key);
            }

            foreach (int i in selectionToDelete)
                selectedNodeIDs.Remove(i);

            if (selectedNodeIDs.Count > 0)
                roadPartSelected = true;
        }


        public void OnDrawGizmos()
        {
            const float roadWidth = 0.5f; // TODO: road width
            
            var oldColor = Gizmos.color;

            RoadGizmos.zOffset = 0.01f;
            
            foreach (RoadNode node in Nodes)
            {
                Gizmos.color = Color.yellow;
                foreach (RoadConnection connection in node.Connections)
                {
                    RoadGizmos.DrawRoadLines(connection.NodeA.WorldPosition, connection.NodeB.WorldPosition, roadWidth);
                }
                
                Gizmos.color = node.Selected ? Color.yellow : Color.cyan;
                RoadGizmos.DrawWireCircle(node.Position, roadWidth / 2f, 20); // TODO: road with
            }

            Gizmos.color = oldColor;
        }
    }
}
