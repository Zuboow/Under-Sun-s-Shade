using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprites : MonoBehaviour
{
    public Sprite
        EmptyFarmingField,
        PlantedWheatField,
        GrowingWheatField,
        GrownWheatField,
        PlantedCarrotField,
        GrowingCarrotField,
        GrownCarrotField,
        PlantedPumpkinField,
        GrowingPumpkinField,
        GrownPumpkinField;

    public enum SeedType
    {
        Wheat,
        Carrot,
        Pumpkin
    }

    public List<Sprite> LoadSprites(SeedType type)
    {
        switch (type)
        {
            case SeedType.Wheat:
                return new List<Sprite>() { PlantedWheatField, GrowingWheatField, GrownWheatField };
            case SeedType.Carrot:
                return new List<Sprite>() { PlantedCarrotField, GrowingCarrotField, GrownCarrotField };
            case SeedType.Pumpkin:
                return new List<Sprite>() { PlantedPumpkinField, GrowingPumpkinField, GrownPumpkinField};
            default:
                return null;
        }
    }
}
