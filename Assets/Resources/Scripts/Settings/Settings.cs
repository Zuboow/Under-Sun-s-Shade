using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static int 
        MaxItemQuantityPerSlot = 99, 
        MaxItemManagementTimes = 100,
        DefenceTurretDamage = 25;
    public static KeyCode 
        InventoryButton = KeyCode.E, 
        UseButton = KeyCode.F, 
        DroppingButton = KeyCode.Q, 
        CraftingViewButton = KeyCode.C,
        BuildingRotationButton = KeyCode.R,
        AutoAddButton = KeyCode.LeftShift;
    public static Vector2
        ClosedInventoryPosition = new Vector2(9999, 9999),
        OpenInventoryPosition = new Vector2(0, 0),
        OpenInventoryPositionWithChestOpen = new Vector2(370, 0),
        OpenCraftingPosition = new Vector2(-480, 0),
        ClosedCraftingPosition = new Vector2(9999, 9999),
        DefenceTurretPositionBottom = new Vector2(0, -0.15f),
        DefenceTurretPositionLeft = new Vector2(-0.5f, 0.15f),
        DefenceTurretPositionTop = new Vector2(0, 0.25f),
        DefenceTurretPositionRight = new Vector2(0.5f, 0.15f),
        DefenceTurretPositionLeftBottom = new Vector2(-0.4f, -0.1f),
        DefenceTurretPositionLeftTop = new Vector2(-0.3f, 0.35f),
        DefenceTurretPositionRightBottom = new Vector2(0.4f, -0.1f),
        DefenceTurretPositionRightTop = new Vector2(0.3f, 0.35f)
        ;
    public static float
        PlayerSpeed = 2.5f,
        ItemSlideSpeed = 5f,
        WaitingTimeAfterDroppingFromInventory = 2f,
        TimeForDeadEntityToBeRemoved = 3f,
        DistanceForItemsToSlideTowardsPlayer = 2f,
        DistanceForItemsToAddToPlayerInventory = 0.2f,
        DistanceForInteractableObjectToDetectHovering = 3f,
        DistanceRangeOfItemDropping = 1f,
        DistanceForObjectsToBeInteractable = 2f,
        DistanceForThingsToBeAudible = 20f,
        DistanceForTurretToDetectEntities = 5f,
        DistanceForAlienToDetectPlayerOrBuildings = 10f,
        MaxDistanceBetweenItemsOnConveyorBelt = 0.22f,
        TurretShootingInterval = 0.1f;
    public static bool 
        CheckCollisionOnAnimationHit = false, 
        CanMove = true;
    public static string
        FarmingFieldEmptyText = "[F] PLANT",
        FarmingFieldHarvestingText = "[F] HARVEST";
    public static OpenInteractableInventoryType InteractableInventoryType = OpenInteractableInventoryType.Empty;
    public static Dictionary<string, string> SmeltingInputAndOutput = new Dictionary<string, string>();
    public static Dictionary<string, float> FuelNameAndTime = new Dictionary<string, float>();

    public enum OpenInteractableInventoryType
    {
        Empty,
        Chest,
        Furnace,
        AssemblyMachine
    }

    private void Start()
    {
        SmeltingInputAndOutput.Add("Iron Ore", "Iron Bar");
        SmeltingInputAndOutput.Add("Copper Ore", "Copper Bar");

        FuelNameAndTime.Add("Coal Ore", 8f);
        FuelNameAndTime.Add("Stick", 3f);
    }
}
