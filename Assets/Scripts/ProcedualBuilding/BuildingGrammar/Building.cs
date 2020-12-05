using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grammar.Building
{
    public class Building
    {
        Vector2Int size;
        Wing[] wings;

        public Vector2Int Size { get { return size; } }
        public Wing[] Wings { get { return wings; } }

        public Building(int sizeX, int sizeY, Wing[] wings)
        {
            size = new Vector2Int(sizeX, sizeY);
            this.wings = wings;
        }

        public override string ToString()
        {
            string building = $"Building:({size}; {wings.Length})\n";

            foreach (Wing wing in wings)
            {
                building += $"\t{wing})";
            }

            return building;
        }
    }
}
