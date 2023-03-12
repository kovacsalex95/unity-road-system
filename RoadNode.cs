using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace unity_road_system
{
    [ExecuteInEditMode]
    public class RoadNode : MonoBehaviour
    {
        public uint ID;
        
        public Vector3 Position => transform.localPosition;
        
        public Vector3 WorldPosition => transform.position;
        
        public RoadSystem ParentSystem => system;

        public bool Selected => selectionID > 0;

        public uint SelectionID => selectionID;
        

        private RoadSystem system;
        
        [SerializeField]
        private List<RoadConnection> connections;

        private uint selectionID;

        private uint[] connectedNodeIDs;
        private static readonly int Selected1 = Shader.PropertyToID("_Selected");


        public uint[] ConnectedNodeIDs { get
        {
            if (connectedNodeIDs == null) connectedNodeIDs = new uint[0];
            return connectedNodeIDs;
        } }

        public bool HasAnyConnection => ConnectedNodeIDs.Length > 0;


        public void Init(RoadSystem parentSystem = null)
        {
            this.system = parentSystem;
            
            if (this.system == null) this.system = GetComponentInParent<RoadSystem>();
            if (this.system == null) this.system = RoadSystem.FirstSystem();

            this.system.OnSystemUpdate += this.OnSystemUpdate;
        }


        private void OnSystemUpdate(object sender, EventArgs e)
        {
            UpdateConnections();
        }

        
        public List<RoadConnection> Connections
        {
            get
            {
                if (connections == null)
                    connections = new List<RoadConnection>();

                return connections;
            }
        }

        
        public void ConnectNode(RoadNode node)
        {
            if (node.ID < ID)
            {
                node.ConnectNode(this);
                return;
            }

            if (HasConnectionWith(node))
            {
                Debug.LogWarning($"These two node is already connected! (#{ID} -> #{node.ID})");
                return;
            }

            RoadConnection connection = new RoadConnection
            {
                NodeA = this,
                NodeB = node
            };
            connection.RecalculateDistance();

            Connections.Add(connection);
            UpdateConnections();
        }


        public void DisconnectNode(RoadNode node)
        {
            List<RoadConnection> connectionsToRemove = new List<RoadConnection>();
            foreach (RoadConnection connection in Connections)
            {
                if (connection.NodeA.ID == node.ID || connection.NodeB.ID == node.ID)
                    connectionsToRemove.Add(connection);
            }

            foreach (RoadConnection c in connectionsToRemove)
                connections.Remove(c);
            
            UpdateConnections();
        }


        private void UpdateConnections()
        {
            List<uint> IDs = new List<uint>();
            
            foreach (KeyValuePair<uint, RoadNode> node in system.Nodes)
            {
                if (node.Value.HasConnectionWith(this) && !IDs.Contains(node.Key))
                    IDs.Add(node.Key);
            }

            foreach (RoadConnection connection in Connections)
            {
                if (IDs.Contains(connection.NodeB.ID))
                    continue;
                
                IDs.Add(connection.NodeB.ID);
            }

            connectedNodeIDs = IDs.ToArray();
        }
        
        
        public bool HasConnectionWith(RoadNode node)
        {
            return this.HasConnectionWith(node.ID);
        }

        
        public bool HasConnectionWith(uint nodeID)
        {
            foreach (RoadConnection connection in Connections)
            {
                if (connection.NodeB.ID == nodeID)
                    return true;
            }

            return false;
        }
        
        
        public void MoveTo(Vector3 newPosition)
        {
            if (newPosition == transform.position)
                return;
            
            transform.position = newPosition;

            foreach (RoadConnection connection in Connections)
            {
                connection.RecalculateDistance();
            }
        }
        
        
        public void Move(Vector3 newPosition)
        {
            this.MoveTo(transform.localPosition + newPosition);
        }


        public void Select()
        {
            this.selectionID = Util.NewID;
            UpdateMaterialSelection();
        }


        public void Deselect()
        {
            selectionID = 0;
            UpdateMaterialSelection();
        }


        private void UpdateMaterialSelection()
        {
            MeshRenderer _renderer = GetComponent<MeshRenderer>();
            if (_renderer == null)
                return;
            
            _renderer.sharedMaterial.SetFloat(Selected1, Selected ? 1f : 0f);
        }


        public override string ToString()
        {
            return $"Road node #{ID}";
        }
    }
}
