using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildings : MonoBehaviour
{
    public static Dictionary<string, string> NameAndPath = new Dictionary<string, string>();

    private void OnEnable()
    {
        NameAndPath.Add("Furnace", "Prefabs/Objects/Furnace");
        NameAndPath.Add("Chest", "Prefabs/Objects/Chest");
        NameAndPath.Add("Defence Turret", "Prefabs/Objects/DefenceTurret");
        NameAndPath.Add("Hydroponic Farm", "Prefabs/Objects/HydroponicFarm");
        NameAndPath.Add("Basic Conveyor Belt", "Prefabs/Objects/BasicConveyorBelt");
        NameAndPath.Add("Assembly Machine", "Prefabs/Objects/AssemblyMachine");
    }
}
