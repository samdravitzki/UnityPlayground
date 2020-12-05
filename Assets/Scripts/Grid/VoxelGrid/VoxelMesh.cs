using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grid.VoxelGrid
{
    public class VoxelMesh
    {
        private List<Vector3> _vertices = null;
        private List<int> _triangles = null; // Indices
        
        private PointField3D _pointField;
        private float _voxelScale = 1f;
        private float AdjustedVoxelScale => _voxelScale * 0.5f;
        
        public VoxelMesh(PointField3D pointField, float voxelScale = 1f)
        {
            _pointField = pointField;
            _voxelScale = voxelScale;
        }

        public void GenerateMesh()
        {
            _vertices = new List<Vector3>();
            _triangles = new List<int>();

            foreach (var x in Enumerable.Range(0, _pointField.Width))
            {
                foreach (var y in Enumerable.Range(0, _pointField.Height))
                {
                    foreach (var z in Enumerable.Range(0, _pointField.Depth))
                    {
                        if (_pointField.GetCell(x, y, z) == 0)
                        {
                            continue;
                        }

                        // Offsets required to center the mesh around the origin
                        float xCenterOffset = (_pointField.Width * _voxelScale) / 2 - AdjustedVoxelScale;
                        float yCenterOffset = (_pointField.Height * _voxelScale) / 2 - AdjustedVoxelScale;
                        float zCenterOffset = (_pointField.Depth * _voxelScale) / 2 - AdjustedVoxelScale;

                        MakeCube(AdjustedVoxelScale,
                            new Vector3(
                                x * _voxelScale - xCenterOffset,
                                y * _voxelScale - yCenterOffset,
                                z * _voxelScale - zCenterOffset),
                            x, y, z);
                    }
                }
            }
        }

        // x, y, z = the voxel data coordinate, cubePosition = the cubes real world position
        // The x, y, z data is required to determine whether a face of the cube should be drawn or not
        void MakeCube(float cubeScale, Vector3 cubePosition, int x, int y, int z)
        {
            foreach (int direction in Enumerable.Range(0, 6)) // 6 faces of a cube
            {
                // Only draw the face of the cube in a given direction if there is nothing (or 0 value) in that direction
                if (_pointField.GetNeighbour(x, y, z, (Direction) direction) == 0)
                {
                    MakeFace((Direction) direction, cubeScale, cubePosition);
                }
            }
        }

        void MakeFace(Direction direction, float faceScale, Vector3 facePosition)
        {
            // Add four _vertices to the list based on the given direction
            _vertices.AddRange(CubeMeshData.FaceVertices(direction, faceScale, facePosition));
            // Configure two _triangles for the four new _vertices
            _triangles.Add(_vertices.Count - 4);
            _triangles.Add(_vertices.Count - 4 + 1);
            _triangles.Add(_vertices.Count - 4 + 2);
            _triangles.Add(_vertices.Count - 4);
            _triangles.Add(_vertices.Count - 4 + 2);
            _triangles.Add(_vertices.Count - 4 + 3);
        }


        public void ApplyMesh(Mesh mesh)
        {

            if (_vertices == null || _triangles == null)
            {
                throw new System.Exception("A voxel mesh must first be calculated");
            }
            
            mesh.Clear();
            mesh.vertices = _vertices.ToArray();
            mesh.triangles = _triangles.ToArray();
            mesh.RecalculateNormals();
        }
    }
}