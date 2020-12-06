using System;
using System.Collections;
using System.Collections.Generic;
using Grid;
using Grid.VoxelGrid;
using Infinity;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Grid.VoxelGrid
{
    // The voxel controller updates a given VoxelRenderers pointfeild based on input
    [RequireComponent(typeof(VoxelRenderer))]
    [RequireComponent(typeof(Infinity.Instancing.GridInstance))]
    public class VoxelController : MonoBehaviour
    {

        private VoxelRenderer voxelRenderer;
        private Infinity.Instancing.GridInstance instancer;
        /* positive values will give you the block position you are hitting,
        negative values will give you the block position adjacent
        to the block you are hitting */
        public float hitOffset = 0.1f;
        
        public GameObject selectorPlane;
        public float selectorOffset = 0.1f;
        
        public Vector3Int selectedGridPos = Vector3Int.zero;

        [SerializeField]
        private Camera _selctionCamera;

        void Awake()
        {
            voxelRenderer = GetComponent<VoxelRenderer>();
            instancer = GetComponent<Infinity.Instancing.GridInstance>();
            selectorPlane.transform.localScale *= voxelRenderer.cellSize;
        }

        void Update()
        {
            // Refactor: Find more modern way to do this later
            Ray ray = _selctionCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 worldClickPos = hit.point + (ray.direction * hitOffset);
                var gridPos = 
                    VoxelUtils.WorldToVoxelSpace(worldClickPos, voxelRenderer.transform.position, voxelRenderer.cellSize);
                var worldGridPos =
                    VoxelUtils.VoxelToWorldSpace(gridPos, voxelRenderer.transform.position, voxelRenderer.cellSize);
                
                selectorPlane.SetActive(true);
                selectorPlane.transform.position = (worldGridPos) + hit.normal * (selectorOffset + voxelRenderer.cellSize * 0.5f);
                selectorPlane.transform.rotation = Quaternion.LookRotation(hit.normal);
                selectedGridPos = VoxelUtils.VoxelToPointFieldSpace(gridPos + VoxelUtils.ToVector3Int(hit.normal), voxelRenderer.pointField.Size);
                
                // Update the mesh after changing the pointfeild
                voxelRenderer.UpdateMesh();
                if (voxelRenderer.VoxelMesh != null)
                    instancer.UpdateMesh(voxelRenderer.VoxelMesh);
            }
            else
            {
                selectorPlane.SetActive(false);
            }
        }

        private void OnMouseDown()
        {
            voxelRenderer.pointField.SetCell(selectedGridPos.x, selectedGridPos.y, selectedGridPos.z, 1);
        }
    }
}

