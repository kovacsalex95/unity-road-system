using UnityEngine;

namespace lxkvcs.UnityRoadSystem
{
    public class RoadNode
    {
        private Vector3 position = Vector3.zero;
        private RoadSystem system; 
        public RoadConnection[] Connections;

        public RoadNode()
        {
            this.system = RoadSystem.FirstSystem();
        }
        
        public RoadNode(RoadSystem system)
        {
            this.system = system;
        }
        
        public RoadNode(RoadSystem system, Vector3 position)
        {
            this.position = position;
            this.system = system;
        }
    }
}