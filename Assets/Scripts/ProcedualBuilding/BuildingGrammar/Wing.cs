using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grammar.Building
{
    public class Wing
    {
        RectInt bounds; // Area the building uses of the buildings size (the RectInt represents the wings min and max bounds)
        Story[] stories;
        Roof roof;

        public RectInt Bounds { get => bounds; }
        public Story[] Stories { get { return stories; } }
        public Roof Roof { get { return roof; } }

        public Wing(RectInt bounds)
        {
            this.bounds = bounds;
        }

        public Wing(RectInt bounds, Story[] stories, Roof roof)
        {
            this.bounds = bounds;
            this.stories = stories;
            this.roof = roof;
        }

        public override string ToString()
        {
            string wing = $"Wing:({bounds})\n";

            foreach (Story story in stories)
            {
                wing += $"\t\t{story})";
            }
            wing += "\n";
            wing += $"\t{roof})";

            return wing;
        }
    }
}
