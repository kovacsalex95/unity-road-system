using UnityEngine;

namespace lxkvcs.UnityRoadSystem
{
    public struct RoadConnection
    {
        public RoadNode NodeA;
        public RoadNode NodeB;
        public float Distance;

        public void RecalculateDistance()
        {
            Distance = Vector3.Distance(NodeA.Position, NodeB.Position);
        }
    }
}
