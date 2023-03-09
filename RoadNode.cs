using UnityEngine;

namespace lxkvcs.UnityRoadSystem
{
    public class RoadNode : MonoBehaviour
    {
        private RoadSystem system = null; 
        public RoadConnection[] Connections;

        public Vector3 Position => transform.localPosition;
        public Vector3 WorldPosition => transform.position;

        public void Init(RoadSystem system = null)
        {
            this.system = system;
            
            if (this.system == null)
                this.system = GetComponentInParent<RoadSystem>();
            if (this.system == null)
                this.system = RoadSystem.FirstSystem();
        }
        
        public void MoveTo(Vector3 newPosition)
        {
            if (newPosition == transform.position)
                return;
            
            // TODO: Recalculate distances
            transform.position = newPosition;
        }
        
        public void Move(Vector3 newPosition)
        {
            this.MoveTo(transform.localPosition + newPosition);
        }
    }
}