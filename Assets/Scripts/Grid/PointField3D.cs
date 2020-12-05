using System.Collections.Generic;
using System.Linq;
using System;
using JetBrains.Annotations;
using UnityEngine;

// Potentially this could be an abstract class for various types of 3D points clouds
// For now this class will just represent a 3D voxel grid
namespace Grid
{
    // Type aliases
    using GenerationRule = Func<int, int, int, bool>;
    
    public class PointField3D
    {
        private int[,,] data;

        public int Width => data.GetLength(0);

        public int Height => data.GetLength(1);

        public int Depth => data.GetLength(2);
        
        public Vector3Int Size => new Vector3Int(Width, Height, Depth);

        public PointField3D(int width, int height, int depth, GenerationRule rule = null)
        {
            if (rule == null)
            {
                rule = (x, y, z) => false;
            }
            data = Generate(width, height, depth, rule);
        }

        // Generate iterates over each point in the point field and applies a given rule to each point
        public int[,,] Generate(int width, int height, int depth, [CanBeNull] GenerationRule rule = null)
        {
            int[,,] generatedData = new int[width, height, depth];

            if (rule == null)
            {
                return generatedData;
            }

            foreach (var x in Enumerable.Range(0, width))
            {
                foreach (var y in Enumerable.Range(0, height))
                {
                    foreach (var z in Enumerable.Range(0, depth))
                    {
                        generatedData[x, y, z] = rule(x, y, z) ? 1 : 0;
                    }
                }
            }

            return generatedData;
        }

        public int GetCell(int x, int y, int z)
        {
            return data[x, y, z];
        }

        public void SetCell(int x, int y, int z, int value)
        {
            data[x, y, z] = value;
        }

        // Find the value of a neighbour of a point in a given direction
        public int GetNeighbour(int x, int y, int z, Direction direction)
        {
            // Derive the offset from the direction supplied
            DataCoordinate offsetToCheck = directionalOffsets[direction];
            // Retrive the coordinate of the neighbour based on the offset derived
            DataCoordinate neighbour = new DataCoordinate(
                x + offsetToCheck.x,
                y + offsetToCheck.y, 
                z + offsetToCheck.z);

            // Check the neighbour is not out of the bounds of the pointField
            if (!CoordInField(neighbour))
            {
                return 0; // Nothing is outside the point field
            }

            return GetCell(neighbour.x, neighbour.y, neighbour.z);
        }
        
        private bool CoordInField(DataCoordinate coord)
        {
            return CoordInField(new Vector3Int(coord.x, coord.y, coord.z));
        }

        public bool CoordInField(Vector3Int coord)
        {
            if (coord.x < 0 || coord.x >= Width
                || coord.y < 0 || coord.y >= Height
                || coord.z < 0 || coord.z >= Depth)
            {
                return false; // Cell is outside the point field
            }

            return true; // Cell is within the point field
        }
        
        // Only for use within this class
        private readonly struct DataCoordinate
        {
            public readonly int x;
            public readonly int y;
            public readonly int z;

            public DataCoordinate(int x, int y, int z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public override string ToString()
            {
                return $"({x}, {y}, {z})";
            }
        }

        // Define a lookup for the offsets relating to a direction, meaning which direction should we look for a neighbour for a given direction
        private Dictionary<Direction, DataCoordinate> directionalOffsets = new Dictionary<Direction, DataCoordinate>()
        {
            [Direction.North] = new DataCoordinate(0, 0, 1),
            [Direction.East] = new DataCoordinate(1, 0, 0),
            [Direction.South] = new DataCoordinate(0, 0, -1),
            [Direction.West] = new DataCoordinate(-1, 0, 0),
            [Direction.Up] = new DataCoordinate(0, 1, 0),
            [Direction.Down] = new DataCoordinate(0, -1, 0)
        };
    }

    public enum Direction
    {
        North,
        East,
        South,
        West,
        Up,
        Down
    }
}


