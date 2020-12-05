using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grammar.Building
{
    public class Roof
    {
        RoofType type;
        RoofDirection direction;

        public RoofType Type { get => type; }
        public RoofDirection Direction { get => direction; }

        public Roof(RoofType type = RoofType.Point, RoofDirection direction = RoofDirection.North)
        {
            this.type = type;
            this.direction = direction;
        }

        public override string ToString()
        {
            return $"Roof: {type}";
        }
    }

    public enum RoofType // Size 8
    {
        Flat,
        Point,
        Slope,
        SlopeWindow,
        Left,
        Right,
        Corner,
        CornerInner,
    }

    public enum RoofDirection
    {
        North, // positive y
        East, // positive x
        South, // negative y
        West // negative x
    }
}
