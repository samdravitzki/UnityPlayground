using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Grammar.Building;

/**
 * Terminology:
 * - Render = Non-Terminal
 * - Place = Terminal
 */

public class BuildingRenderer : MonoBehaviour
{
    public Transform floorPrefab;
    public Transform[] wallPrefab;
    public Transform[] roofPrefab;
    Transform buildingParent;

    private const float wallOffsetNorth = +0.05f;
    private const float wallOffsetEast = +0.95f;
    private const float wallOffsetSouth = -0.95f;
    private const float wallOffsetWest = -0.05f;

  
    
    public void Render(Building building)
    {
        buildingParent = new GameObject("Building").transform;
        foreach(Wing wing in building.Wings)
        {
            RenderWing(wing, buildingParent);
        }
    }

    private void RenderWing(Wing wing, Transform buildingParent)
    {
        Transform wingParent = new GameObject("Wing").transform;
        wingParent.SetParent(buildingParent);
        foreach (Story story in wing.Stories)
        {
            RenderStory(story, wing, wingParent);
        }
        RenderRoof(wing, wingParent);
    }

    private void RenderStory(Story story, Wing wing, Transform wingParent)
    {
        Transform storyParent = new GameObject("Story").transform;
        storyParent.SetParent(wingParent);

        foreach(int x in Enumerable.Range(wing.Bounds.min.x, wing.Bounds.max.x))
        {
            foreach (int y in Enumerable.Range(wing.Bounds.min.y, wing.Bounds.max.y))
            {
                // Place floor at xy position
                PlaceFloor(x, y, story.Level, storyParent);

                // Place wall at xy position the position is on the perimeter

                //south wall
                if (y == wing.Bounds.min.y)
                {
                    Transform wall = wallPrefab[(int)story.Walls[x - wing.Bounds.min.x]];
                    wall.name = "South";
                    PlaceSouthWall(x, y, story.Level, storyParent, wall);
                }

                //east wall
                if (x == wing.Bounds.min.x + wing.Bounds.size.x - 1)
                {
                    Transform wall = wallPrefab[(int)story.Walls[wing.Bounds.size.x + y - wing.Bounds.min.y]];
                    wall.name = "East";
                    PlaceEastWall(x, y, story.Level, storyParent, wall);
                }

                //north wall
                if (y == wing.Bounds.min.y + wing.Bounds.size.y - 1)
                {
                    Transform wall = wallPrefab[(int)story.Walls[wing.Bounds.size.x * 2 + wing.Bounds.size.y - (x - wing.Bounds.min.x + 1)]];
                    wall.name = "North";
                    PlaceNorthWall(x, y, story.Level, storyParent, wall);
                }

                //west wall
                if (x == wing.Bounds.min.x)
                {
                    Transform wall = wallPrefab[(int)story.Walls[(wing.Bounds.size.x + wing.Bounds.size.y) * 2 - (y - wing.Bounds.min.y + 1)]];
                    wall.name = "West";
                    PlaceWestWall(x, y, story.Level, storyParent, wall);
                }
            }
        }
    }
    private void RenderRoof(Wing wing, Transform wingParent)
    {
        foreach (int x in Enumerable.Range(wing.Bounds.min.x, wing.Bounds.max.x))
        {
            foreach (int y in Enumerable.Range(wing.Bounds.min.y, wing.Bounds.max.y))
            {
                PlaceRoof(x, y, wing.Stories.Length, wingParent, wing.Roof.Type, wing.Roof.Direction);
            }
        }
    }

    private void PlaceFloor(int x, int y, int level, Transform storyParent)
    {
        Transform floor = Instantiate(
            floorPrefab,
            storyParent.TransformPoint(new Vector3(x, level, y)), 
            Quaternion.identity );

        floor.SetParent(storyParent);
    }



    private void PlaceNorthWall(int x, int y, int level, Transform storyFolder, Transform wall)
    {
        Transform w = Instantiate(
            wall,
            storyFolder.TransformPoint(
                new Vector3(
                    x,
                    level,
                    y + wallOffsetNorth
                    )
                ),
            Quaternion.Euler(0, 90, 0));
        w.SetParent(storyFolder);
    }

    private void PlaceEastWall(int x, int y, int level, Transform storyFolder, Transform wall)
    {
        Transform w = Instantiate(
            wall,
            storyFolder.TransformPoint(
                new Vector3(
                    x + wallOffsetEast,
                    level,
                    y
                    )
                ),
            Quaternion.identity);
        w.SetParent(storyFolder);
    }


    private void PlaceSouthWall(int x, int y, int level, Transform storyFolder, Transform wall)
    {
        Transform w = Instantiate(
            wall,
            storyFolder.TransformPoint(
                new Vector3(
                    x,
                    level,
                    y + wallOffsetSouth
                    )
                ),
            Quaternion.Euler(0, 90, 0));
        w.SetParent(storyFolder);
    }


    private void PlaceWestWall(int x, int y, int level, Transform storyFolder, Transform wall)
    {
        Transform w = Instantiate(
            wall,
            storyFolder.TransformPoint(
                new Vector3(
                    x + wallOffsetWest,
                    level,
                    y
                    )
                ),
            Quaternion.identity);
        w.SetParent(storyFolder);
    }

    private void PlaceRoof(int x, int y, int topLevel, Transform wingParent, RoofType type, RoofDirection direction)
    {
        Transform roof = Instantiate(
            roofPrefab[(int)type],
            wingParent.TransformPoint(
                new Vector3(
                    x,
                    topLevel,
                    y
                )
            ),
            Quaternion.Euler(0f, rotationOffset[(int)direction].y, 0f)
        );

        roof.SetParent(wingParent);
    }

    // The rotation to give a roof based on its direction
    Vector3[] rotationOffset = {
        new Vector3 (-3f, 270f, 0f),
        new Vector3 (0f, 0f, 0f),
        new Vector3 (0f, 90, -3f),
        new Vector3 (-3f, 180, -3f)
    };
}
