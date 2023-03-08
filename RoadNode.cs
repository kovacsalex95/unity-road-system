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

        public Vector3 Position => position;

        public void MoveTo(Vector3 newPosition)
        {
            if (newPosition == position)
                return;
            
            // TODO: Recalculate distances
            this.position = newPosition;
        }
        
        public void Move(Vector3 newPosition)
        {
            this.MoveTo(position + newPosition);
        }
        
        public Vector3 WorldPosition => position + system.transform.TransformPoint(position);
    }
}