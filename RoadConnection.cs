using UnityEngine;


namespace unity_road_system
{
    [System.Serializable]
    public struct RoadConnection
    {
        [SerializeReference]
        public RoadNode NodeA;
        [SerializeReference]
        public RoadNode NodeB;
        public float Distance;

        public void RecalculateDistance()
        {
            Distance = Vector3.Distance(NodeA.Position, NodeB.Position);
        }
    }
}
