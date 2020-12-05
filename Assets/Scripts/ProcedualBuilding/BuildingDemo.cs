using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grammar.Building;

public class BuildingDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Building building = BuildingGenerator.Generate();
        GetComponent<BuildingRenderer>().Render(building);
        Debug.Log(building.ToString());
    }
}
