using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace lxkvcs.UnityRoadSystem
{
    public class RoadSystem : MonoBehaviour
    {
        const float NEW_NODE_DISTANCE = 2f; // in units (m)
        
        
        [SerializeField]
        public bool enableSnapping = true;
        [SerializeField]
        public float snappingDistance = 0.25f;
        
        
        private List<RoadNode> nodes = null;

        
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
                }

                return nodes;
            }
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
            
            Nodes.Add(node);
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
    }
}
