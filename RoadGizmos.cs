using UnityEngine;

namespace lxkvcs.UnityRoadSystem
{
    public class RoadGizmos
    {
        public static float zOffset = 0.001f;
        
        
        public static void DrawWireCircle(Vector3 center, float width, int segments = 10)
        {
            if (segments < 3)
                return;
            
            float degree = 360f / segments;

            for (int i = 1; i <= segments; i++)
            {
                float radA = (i - 1) * degree * Mathf.Deg2Rad;
                float radB = i * degree * Mathf.Deg2Rad;

                Vector3 pointA = center + new Vector3(Mathf.Sin(radA), zOffset, Mathf.Cos(radA)) * width;
                Vector3 pointB = center + new Vector3(Mathf.Sin(radB), zOffset, Mathf.Cos(radB)) * width;
                
                Gizmos.DrawLine(pointA, pointB);
            }
        }


        public static void DrawRoadLines(Vector3 pointA, Vector3 pointB, float roadWidth = 0.5f)
        {
            Vector3 direction = (pointB - pointA).normalized;
            Vector3 left = Vector3.Cross(direction, Vector3.up) * (roadWidth / 2f);
            Vector3 right = -left;
            Gizmos.DrawLine(pointA + left + Vector3.up * zOffset, pointB + left + Vector3.up * zOffset);
            Gizmos.DrawLine(pointA + right + Vector3.up * zOffset, pointB + right + Vector3.up * zOffset);
        }
    }
}