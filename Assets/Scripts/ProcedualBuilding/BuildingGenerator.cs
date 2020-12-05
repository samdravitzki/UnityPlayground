using UnityEngine;
using Grammar.Building;

public static class BuildingGenerator
{
    public static Building Generate()
    {
        return new Building(4, 4, new Wing[] {
            new Wing(new RectInt(0, 0, 4, 4), new Story[]
            {
                new Story(0, new Wall[((4+4)*2)]),
                new Story(1, new Wall[((4+4)*2)])
            }, 
            new Roof(RoofType.Point, RoofDirection.North))
        });
    }
}
