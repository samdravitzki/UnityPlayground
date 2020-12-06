using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infinity;
using JetBrains.Annotations;
using UnityEngine;


namespace Infinity.Instancing
{

    public class GridInstance : MonoBehaviour
    {
        [Range(0, 10)]
        [SerializeField] private int instancesPerAxis;
        [SerializeField] private Mesh instancedMesh;
        [SerializeField] private Material instancableMaterial;
        
        // Currently only supports one batch with up to 1024 instances
        private List<InstanceTransform> batch;
        
        // Instance in the grid will be spaced out by the given bounds
        public LoopConfiguration settings;
        public LoopConfiguration Settings =>
            settings
                ? settings
                : (settings = ScriptableObject.CreateInstance<LoopConfiguration>());

        private Bounds LoopBounds => Settings.bounds;

        private void Start()
        {
            if (instancableMaterial.enableInstancing == false)
                throw new Exception("GridInstance requires the material to have instancing enabled");
            
            if (instancesPerAxis > 10)
                throw new Exception("GridInstance does not support more than 1023 instances in total (so not more than 10 instances per axis)");

            batch = BuildInstanceBatch();
        }

        private void Update()
        {
            if (instancedMesh == null)
                return;
            RenderBatch();
        }

        private List<InstanceTransform> BuildInstanceBatch()
        {
            var xDiff = LoopBounds.max.x - LoopBounds.min.x;
            var yDiff = LoopBounds.max.y - LoopBounds.min.y;
            var zDiff = LoopBounds.max.z - LoopBounds.min.z;
            var boundStep = new Vector3(xDiff, yDiff, zDiff);
            var centerOffset = (boundStep * instancesPerAxis) / 2;
            
            var instanceTransforms = new List<InstanceTransform>();
            
            foreach (var x in Enumerable.Range(0, instancesPerAxis))
            {
                foreach (var y in Enumerable.Range(0, instancesPerAxis))
                {
                    foreach (var z in Enumerable.Range(0, instancesPerAxis))
                    {
                        var center = (instancesPerAxis / 2);
                        if (x == center && y == center && z == center)
                            continue;
                        
                        instanceTransforms.Add(new InstanceTransform
                        {
                            translation = new Vector3(x * xDiff, y * yDiff, z * zDiff) - centerOffset,
                            scale = Vector3.one,
                            rotation = Quaternion.identity
                        });
                    }
                }
            }

            return instanceTransforms;
        }

        private void RenderBatch()
        {
            Graphics.DrawMeshInstanced(
                mesh: instancedMesh, 
                submeshIndex: 0, 
                material: instancableMaterial,
                matrices: batch.Select(t => t.matrix).ToList());
        }
        
        // Mesh is rendered by a different monobehaviour
        public void UpdateMesh(Mesh mesh)
        {
            instancedMesh = mesh;
        }
    }

    struct InstanceTransform
    {
        public Vector3 translation;
        public Quaternion rotation;
        public Vector3 scale;
        
        public Matrix4x4 matrix => 
            Matrix4x4.TRS(translation, rotation, scale);
    }
}
