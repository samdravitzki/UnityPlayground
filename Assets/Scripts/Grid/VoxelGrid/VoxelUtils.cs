using System;
using UnityEngine;
using Unity.Mathematics;

namespace Grid.VoxelGrid
{
    public static class VoxelUtils
    {
        // Returns the center of a voxel in world coordinates
        public static Vector3 VoxelToWorldSpace(Vector3Int gridPosition, Vector3 chunkPosition, float cellSize)
        {
            return (chunkPosition + gridPosition) * cellSize + new Vector3(cellSize * 0.5f, cellSize * 0.5f, cellSize * 0.5f);
        }
        
        public static Vector3Int WorldToVoxelSpace(Vector3 worldPosition, Vector3 chunkPosition, float cellSize)
        {
            return ToVector3Int(WorldToVoxelSpace(Floor(worldPosition / cellSize), Floor(chunkPosition / cellSize)));
        }

        private static int3 WorldToVoxelSpace(int3 worldGridPosition, int3 chunkPosition)
        {
            return worldGridPosition - chunkPosition;
        }

        public static Vector3Int VoxelToPointFieldSpace(Vector3Int voxelSpacePosition, Vector3Int pointFieldSize)
        {
            return voxelSpacePosition + (pointFieldSize / 2);
        }

        public static Vector3Int PointFieldToVoxelSpace(Vector3Int pointFieldPosition, Vector3Int pointFieldSize)
        {
            return (pointFieldPosition - pointFieldSize / 2);
        }
        
        
        public static Vector3Int ToVector3Int(int3 v) => new Vector3Int(v.x, v.y, v.z);
        public static Vector3Int ToVector3Int(Vector3 v) => ToVector3Int(ToInt3(v));
        public static int3 ToInt3(Vector3Int v) => new int3(v.x, v.y, v.z);
        public static int3 ToInt3(Vector3 v) => Floor(v);
        public static int3 ToGridPosition(Vector3 v, float cellSize) => Floor(v / cellSize);

        // Mod of a given int
        public static int Mod(int v, int m) 
        {
            int r = v%m;
            return r<0 ? r+m : r;
        }
        
        // Mod of a given int3
        public static int3 Mod(int3 v, int3 m)
        {
            return new int3
            {
                x = Mod(v.x, m.x),
                y = Mod(v.y, m.y),
                z = Mod(v.z, m.z)
            };
        }
        
        // Floor a float3
        public static int3 Floor(float3 v)
        {
            return new int3
            {
                x = Floor(v.x),
                y = Floor(v.y),
                z = Floor(v.z)
            };
        }
        
        // Floor a float
        public static int Floor(float x)
        {
            int xi = (int)x;
            return x < xi ? xi - 1 : xi;
        }
    
        public static int3 InclusiveFloor(float3 v)
        {
            return new int3
            {
                x = InclusiveFloor(v.x),
                y = InclusiveFloor(v.y),
                z = InclusiveFloor(v.z)
            };
        }
        
        public static int InclusiveFloor(float x)
        {
            int xint = (int) x;
            return x.Equals(xint) ? xint : Floor(x);
        }
    }
}