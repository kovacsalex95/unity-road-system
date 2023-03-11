using UnityEngine;

namespace lxkvcs.UnityRoadSystem
{
    public class Geometry
    {
        public static Mesh GenerateSphere(int numSegments = 24, int numLayers = 12, float radius = 1f)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Sphere";

            Vector3[] vertices = new Vector3[(numSegments + 1) * (numLayers + 1)];
            Vector3[] normals = new Vector3[vertices.Length];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[vertices.Length * 6];

            int vertexIndex = 0;
            for (int layer = 0; layer <= numLayers; layer++)
            {
                float layerAngle = Mathf.PI / 2 - layer * Mathf.PI / numLayers;
                float layerRadius = radius * Mathf.Cos(layerAngle);
                float layerHeight = radius * Mathf.Sin(layerAngle);

                for (int segment = 0; segment <= numSegments; segment++)
                {
                    float segmentAngle = 2 * Mathf.PI * segment / numSegments;
                    float x = layerRadius * Mathf.Cos(segmentAngle);
                    float z = layerRadius * Mathf.Sin(segmentAngle);

                    vertices[vertexIndex] = new Vector3(x, layerHeight, z);
                    normals[vertexIndex] = vertices[vertexIndex].normalized;
                    uv[vertexIndex] = new Vector2((float)segment / numSegments, (float)layer / numLayers);

                    if (layer < numLayers && segment < numSegments)
                    {
                        int index = layer * (numSegments + 1) + segment;
                        triangles[vertexIndex * 6] = index;
                        triangles[vertexIndex * 6 + 1] = index + 1;
                        triangles[vertexIndex * 6 + 2] = index + numSegments + 1;
                        triangles[vertexIndex * 6 + 3] = index + 1;
                        triangles[vertexIndex * 6 + 4] = index + numSegments + 2;
                        triangles[vertexIndex * 6 + 5] = index + numSegments + 1;
                    }

                    vertexIndex++;
                }
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uv;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();
            
            return mesh;
        }
    }
}