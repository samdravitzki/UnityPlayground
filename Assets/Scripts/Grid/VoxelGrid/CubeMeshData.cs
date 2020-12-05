using UnityEngine;
using System.Linq;

namespace Grid.VoxelGrid
{
    public static class CubeMeshData
    {
        private const int VERTICES_PER_FACE = 4;
        
        // The 8 vertices required for a cube mesh (In this case only the north and south quad data is needed as the rest can be inferred)
        private static readonly Vector3[] vertices =
        {
            new Vector3(1, 1, 1),
            new Vector3(-1,1, 1),
            new Vector3(-1,-1, 1),
            new Vector3(1,-1, 1),
            new Vector3(-1,1, -1),
            new Vector3(1,1, -1),
            new Vector3(1,-1, -1),
            new Vector3(-1,-1, -1)
        };
        
        // The vertex order required for each face (where indexes line up the the vertices array above)
        private static readonly int[][] faceTriangles =
        {
            new[] {0, 1, 2, 3}, // North
            new[] {5, 0, 3, 6}, // East
            new[] {4, 5, 6, 7}, // South
            new[] {1, 4, 7, 2}, // West
            new[] {5, 4, 1, 0}, // Up
            new[] {3, 2, 7, 6} // Down
        };
        
        // Return the vertices related to a given array in the triangles array
        // direction = the integer relating to a given direction (0 = North, 1 = East, 2 = South, 3 = West, 4 = Up, 5 = Down)
        public static Vector3[] FaceVertices(int direction, float scale, Vector3 position)
        {
            Vector3[] faceVertices = new Vector3[VERTICES_PER_FACE];
            foreach (int index in Enumerable.Range(0, faceVertices.Length))
            {
                faceVertices[index] = (vertices[faceTriangles[direction][index]] * scale) + position;
            }

            return faceVertices;
        }
        
        public static Vector3[] FaceVertices(Direction direction, float scale, Vector3 position)
        {
            return FaceVertices((int) direction, scale, position);
        }
    }
}