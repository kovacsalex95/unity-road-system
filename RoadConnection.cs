using UnityEngine;

namespace lxkvcs.UnityRoadSystem
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
