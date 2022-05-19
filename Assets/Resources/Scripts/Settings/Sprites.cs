using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprites : MonoBehaviour
{
    public static Dictionary<BuildingController.BuildingRotation, string> ConveyorBelts = new Dictionary<BuildingController.BuildingRotation, string>();
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

    private void OnEnable()
    {
        ConveyorBelts.Add(BuildingController.BuildingRotation.Bottom, "BasicConveyorBelt_Bottom");
        ConveyorBelts.Add(BuildingController.BuildingRotation.Top, "BasicConveyorBelt_Top");
        ConveyorBelts.Add(BuildingController.BuildingRotation.Right, "BasicConveyorBelt_Right");
        ConveyorBelts.Add(BuildingController.BuildingRotation.Left, "BasicConveyorBelt_Left");
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
