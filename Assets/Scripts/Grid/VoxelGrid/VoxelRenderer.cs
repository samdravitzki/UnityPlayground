using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Grid.VoxelGrid
{
    // Future Refactor: Factor rendering component into its own class, then create Renderer author component with point feild, cellsize and offset seperated
    // Future Refactor: remove all reference to point feild from this class
    // Render Voxels to a point feild
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


        void Awake()
        {
            mesh = GetComponent<MeshFilter>().sharedMesh;
            _collider = GetComponent<MeshCollider>();
        }

        void Start()
        {
            pointField = new PointField3D(
                size.x, size.y, size.z,
                (x, y, z) => y == 0 // && x % 2 == 0 && z % 2 == 0// && (x >= 4 && x <= 6 && z >= 4 && z <= 6)
            );

        }
        
        
        public void Update()
        {
            var meshVox = new VoxelMesh(pointField, cellSize);
            meshVox.GenerateMesh();
            meshVox.ApplyMesh(mesh);
            _collider.sharedMesh = mesh;
        }
    }
}
