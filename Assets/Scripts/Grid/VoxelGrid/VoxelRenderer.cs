using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Grid.VoxelGrid
{
    // Renders a given point feild as a voxel mesh
    [ExecuteInEditMode] // Sureley there is a better solution where I can click a button or something in the future
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class VoxelRenderer : MonoBehaviour // Renders a chunk based on a given point feild
    {
        private Mesh mesh;
        private MeshCollider _collider;
        private List<Vector3> vertices;
        private List<int> triangles; // Indices
        
        // There is one pointfield for every renderer
        [NonSerialized] public PointField3D pointField;
        public Vector3Int size;
        public float cellSize = 1f;

        public Mesh VoxelMesh => mesh;
        void Awake()
        {
            mesh = GetComponent<MeshFilter>().sharedMesh = new Mesh();
            mesh.name = "Voxel Mesh";
            _collider = GetComponent<MeshCollider>();
        }

        void Start()
        {
            pointField = new PointField3D(
                size.x, size.y, size.z,
                (x, y, z) => y == 0 // && x % 2 == 0 && z % 2 == 0// && (x >= 4 && x <= 6 && z >= 4 && z <= 6)
            );
            UpdateMesh();
        }
        
        
        public void UpdateMesh()
        {
            var meshVox = new VoxelMesh(pointField, cellSize);
            meshVox.GenerateMesh();
            meshVox.ApplyMesh(mesh);
            _collider.sharedMesh = mesh;
        }
    }
}
