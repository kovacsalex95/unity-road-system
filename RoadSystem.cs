using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace lxkvcs.UnityRoadSystem
{
    public class RoadSystem : MonoBehaviour
    {
        public static RoadSystem FirstSystem()
        {
            RoadSystem[] objects = FindObjectsOfType<RoadSystem>();
            if (objects.Length == 0)
                return null;

            return objects[0];
        }
        
        
    }
}
