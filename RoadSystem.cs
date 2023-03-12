// BUG: Selection stuck after some actions
// BUG: Undo not working properly
// TODO: Create nodes from the scene view


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;


namespace unity_road_system
{
    [ExecuteInEditMode]
    public class RoadSystem : MonoBehaviour
    {
        const float NEW_NODE_DISTANCE = 2f; // in units (m)

        
        public event EventHandler OnSystemUpdate; 
        
        public uint[] selectedNodeIDs = Array.Empty<uint>();
        
        public uint LastNodeID => Nodes.Keys.Last();
        

        private Dictionary<uint, RoadNode> nodes;
        
        private Dictionary<int, uint> selectedNodeDictionary = new();

        
        public Dictionary<uint, RoadNode> Nodes
        {
            get
            {
                if (nodes == null)
                {
                    nodes = new Dictionary<uint, RoadNode>();
                    
                    RoadNode[] childNodes = GetComponentsInChildren<RoadNode>();
                    
                    foreach(RoadNode node in childNodes)
                        nodes.Add(node.ID, node);

                    foreach (KeyValuePair<uint, RoadNode> node in nodes)
                    {
                        node.Value.Init(this);
                    }
                    
                    UpdateSystem();
                }

                return nodes;
            }
        }
        
        
        public void ReloadNodes()
        {
            Nodes.Clear();
            selectedNodeDictionary.Clear();
            nodes = null;
            UpdateSystem();
        }


        public void CreateNode()
        {
            Vector3 position = Vector3.zero;

            if (Nodes.Count > 0)
            {
                position = nodes[LastNodeID].Position;
                float randomDegree = Random.Range(0f, 360f);
                float randomX = Mathf.Sin(randomDegree * Mathf.Deg2Rad);
                float randomY = Mathf.Cos(randomDegree * Mathf.Deg2Rad);
                position += new Vector3(randomX * NEW_NODE_DISTANCE, 0, randomY * NEW_NODE_DISTANCE);
            }

            uint nodeID = Util.NewID;
            
            GameObject nodeObject = new GameObject();
            nodeObject.transform.parent = transform;
            nodeObject.transform.localEulerAngles = Vector3.zero;
            nodeObject.transform.localScale = Vector3.one;
            nodeObject.name = $"Node #{nodeID}";

            RoadNode node = nodeObject.AddComponent<RoadNode>();
            node.Init(this);
            node.MoveTo(position);
            node.ID = nodeID;

            var meshFilter = nodeObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = Resources.RoadNodeMesh;
            var meshRenderer = nodeObject.AddComponent<MeshRenderer>();
            meshRenderer.receiveShadows = false;
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.sharedMaterial = new Material(Resources.RoadNodeMaterial);

            Nodes.Add(node.ID, node);

            UpdateSystem();
        }


        public void ConnectSelectedNodes()
        {
            if (selectedNodeIDs.Length < 2)
                return;
            
            uint[] selectedIDs = selectedNodeIDs;
            for (int i=1; i<selectedIDs.Length; i++)
            {
                Nodes[selectedIDs[i]].ConnectNode(Nodes[selectedIDs[i - 1]]);
            }
            
            UpdateSystem();
        }


        public void DisconnectSelectedNodes()
        {
            if (selectedNodeIDs.Length < 1)
                return;
            
            foreach (uint id in selectedNodeIDs)
            {
                foreach(KeyValuePair<uint, RoadNode> node in Nodes)
                    node.Value.DisconnectNode(Nodes[id]);
            }

            UpdateSystem();
        }


        public void UpdateSystem()
        {
            // TODO: Updates etc
            
            if (OnSystemUpdate != null)
                OnSystemUpdate.Invoke(this, new EventArgs());
        }
        

        public static RoadSystem FirstSystem()
        {
            RoadSystem[] objects = FindObjectsOfType<RoadSystem>();
            if (objects.Length == 0)
                return null;

            return objects[0];
        }


        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        
        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        
        internal void OnSelectionChanged()
        {
            try
            {
                List<int> checkedInstanceIDs = new List<int>();

                foreach (var obj in Selection.objects)
                {
                    if (!obj is GameObject)
                        continue;

                    if (((GameObject)obj).GetComponent<RoadNode>() == null)
                        continue;

                    int instanceID = ((GameObject)obj).GetInstanceID();
                    checkedInstanceIDs.Add(instanceID);

                    if (!selectedNodeDictionary.ContainsKey(instanceID))
                    {
                        selectedNodeDictionary.Add(instanceID, ((GameObject)obj).GetComponent<RoadNode>().ID);
                        ((GameObject)obj).GetComponent<RoadNode>().Select();
                    }
                }

                List<int> selectionToDelete = new List<int>();
                foreach (KeyValuePair<int, uint> ids in selectedNodeDictionary)
                {
                    if (checkedInstanceIDs.Contains(ids.Key))
                        continue;

                    Nodes[ids.Value].Deselect();
                    selectionToDelete.Add(ids.Key);
                }

                foreach (int i in selectionToDelete)
                    selectedNodeDictionary.Remove(i);
            }
            
            // Node deleted
            catch (Exception error)
            {
                ReloadNodes();
                Selection.activeObject = null;
                
                Debug.LogError(error.Message);
            }


            // Update the selected IDs array (sorted by selection ID)
            Dictionary<uint, uint> sortedIDs = new Dictionary<uint, uint>();
            foreach (KeyValuePair<int, uint> nodeID in selectedNodeDictionary)
            {
                RoadNode node = Nodes[nodeID.Value];
                sortedIDs.Add(node.SelectionID, node.ID);
            }
            selectedNodeIDs = sortedIDs.OrderBy(kvp => kvp.Key)
                .Select(kvp => kvp.Value)
                .ToArray();
        }


        public void OnDrawGizmos()
        {
            const float roadWidth = 0.5f; // TODO: road width
            
            var oldColor = Gizmos.color;

            RoadGizmos.zOffset = 0.01f;
            
            foreach (KeyValuePair<uint, RoadNode> node in Nodes)
            {
                Gizmos.color = Color.yellow;
                foreach (RoadConnection connection in node.Value.Connections)
                {
                    RoadGizmos.DrawRoadLines(connection.NodeA.WorldPosition, connection.NodeB.WorldPosition, roadWidth);
                }
                
                Gizmos.color = node.Value.Selected ? Color.yellow : Color.cyan;
                RoadGizmos.DrawWireCircle(node.Value.Position, roadWidth / 2f, 20); // TODO: road with
            }

            Gizmos.color = oldColor;
        }
    }
}
