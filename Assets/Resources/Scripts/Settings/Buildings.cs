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
        NameAndPath.Add("DefenceTurret", "Prefabs/Objects/DefenceTurret");
    }
}
