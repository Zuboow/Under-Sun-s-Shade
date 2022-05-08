using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLists : MonoBehaviour
{
    public static List<string>
        WheatField = new List<string>() { "Wheat", "Wheat", "Wheat", "Wheat" },
        CarrotField = new List<string>() { "Carrot", "Carrot", "Carrot", "Carrot" },
        PumpkinField = new List<string>() { "Pumpkin" }
        ;
    public static int[,]
        WheatFieldSeeds = new int[,] { { 5, 100 }, { 5, 6 } },
        CarrotFieldSeeds = new int[,] { { 8, 100 }, { 8, 6 } },
        PumpkinFieldSeeds = new int[,] { { 10, 100 }, { 10, 6 } };
}
