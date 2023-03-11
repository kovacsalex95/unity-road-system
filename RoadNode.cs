using System;
using System.Collections.Generic;
using UnityEngine;

namespace lxkvcs.UnityRoadSystem
{
    public class RoadNode : MonoBehaviour
    {
        public int ID = -1;

        private RoadSystem system = null;
        public RoadSystem ParentSystem => system;
        
        [SerializeField]
        private List<RoadConnection> connections = null;
        
        public Vector3 Position => transform.localPosition;
        
        public Vector3 WorldPosition => transform.position;

        private uint selectionID = 0;

        public bool Selected => selectionID > 0;

        public uint SelectionID => selectionID;

        
        public List<RoadConnection> Connections
        {
            get
            {
                if (connections == null)
                    connections = new List<RoadConnection>();

                return connections;
            }
        }

        
        public bool ConnectNode(RoadNode node)
        {
            if (node.ID < ID)
                return node.ConnectNode(this);
            
            if (HasConnectionWith(node))
            {
                Debug.LogWarning($"These two node is already connected! (#{ID} -> #{node.ID})");
                return false;
            }

            RoadConnection connection = new RoadConnection
            {
                NodeA = this,
                NodeB = node
            };
            connection.RecalculateDistance();

            Connections.Add(connection);
            return true;
        }

        
        public bool HasConnectionWith(RoadNode node)
        {
            return this.HasConnectionWith(node.ID);
        }

        
        public bool HasConnectionWith(int nodeID)
        {
            foreach (RoadConnection connection in Connections)
            {
                if (connection.NodeB.ID == nodeID)
                    return true;
            }

            return false;
        }


        public void Init(RoadSystem system = null)
        {
            this.system = system;
            
            if (this.system == null) this.system = GetComponentInParent<RoadSystem>();
            if (this.system == null) this.system = RoadSystem.FirstSystem();
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
            
            _renderer.sharedMaterial.SetFloat("_Selected", Selected ? 1f : 0f);
        }


        public override string ToString()
        {
            return $"Road node #{ID}";
        }
    }
}
