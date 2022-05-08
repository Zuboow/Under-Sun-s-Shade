using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FarmingFieldController : MonoBehaviour
{
    public Sprite Empty, SeedsPlanted, Growing, ReadyToHarvest;
    List<string> ItemsToDrop = new List<string>();
    public int TicksNeededForNextStage, Ticks = 0;
    public FarmStage FieldStage;
    public Sprites.SeedType Plant;

    public enum FarmStage
    {
        Empty,
        SeedsPlanted,
        Growing,
        ReadyToHarvest
    }

    public void OnMouseOver()
    {
        if (Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) < Settings.DistanceForInteractableObjectToDetectHovering)
        {
            switch (FieldStage)
            {
                case FarmStage.Empty:
                    if (InventoryController.ActiveHotbarSlot.GetComponent<InventorySlotController>().SlotItem != null && InventoryController.ActiveHotbarSlot.GetComponent<InventorySlotController>().SlotItem.IsSeed)
                    {
                        transform.GetChild(0).gameObject.SetActive(true);
                        transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = Settings.FarmingFieldEmptyText;

                        if (Input.GetKeyDown(Settings.UseButton))
                        {
                            LoadSprites();
                            InventoryController.RemoveItemQuantityFromSlot(InventoryController.ActiveHotbarSlot, 1);
                            SetNextStage();
                        }
                    }
                    break;

                case FarmStage.SeedsPlanted: case FarmStage.Growing:

                    transform.GetChild(0).gameObject.SetActive(false);
                    break;

                case FarmStage.ReadyToHarvest:

                    transform.GetChild(0).gameObject.SetActive(true);
                    transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = Settings.FarmingFieldHarvestingText;

                    if (Input.GetKeyDown(Settings.UseButton))
                    {
                        DropItemsAndSetNextStage();
                    }
                    break;
            }
            if (FieldStage != FarmStage.ReadyToHarvest && (InventoryController.ActiveHotbarSlot.GetComponent<InventorySlotController>().SlotItem == null || !InventoryController.ActiveHotbarSlot.GetComponent<InventorySlotController>().SlotItem.IsSeed))
            {
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    private void OnMouseExit()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void LoadSprites()
    {
        List<Sprite> sprites = new List<Sprite>();
        ItemsToDrop = new List<string>();

        switch (InventoryController.ActiveHotbarSlot.GetComponent<InventorySlotController>().SlotItem.ItemName)
        {
            case "Wheat Seeds":
                sprites.AddRange(GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<Sprites>().LoadSprites(Sprites.SeedType.Wheat));
                Plant = Sprites.SeedType.Wheat;
                ItemsToDrop.AddRange(DropLists.WheatField);
                break;
            case "Carrot Seeds":
                sprites.AddRange(GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<Sprites>().LoadSprites(Sprites.SeedType.Carrot));
                Plant = Sprites.SeedType.Carrot;
                ItemsToDrop.AddRange(DropLists.CarrotField);
                break;
            case "Pumpkin Seeds":
                sprites.AddRange(GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<Sprites>().LoadSprites(Sprites.SeedType.Pumpkin));
                Plant = Sprites.SeedType.Pumpkin;
                ItemsToDrop.AddRange(DropLists.PumpkinField);
                break;
        }
        SeedsPlanted = sprites[0];
        Growing = sprites[1];
        ReadyToHarvest = sprites[2];
    }

    public int[,] LoadSeedPercentages()
    {
        switch (Plant)
        {
            case Sprites.SeedType.Wheat:
                return DropLists.WheatFieldSeeds;
            case Sprites.SeedType.Carrot:
                return DropLists.CarrotFieldSeeds;
            case Sprites.SeedType.Pumpkin:
                return DropLists.PumpkinFieldSeeds;
            default:
                return null;
        }
    }

    public void DropItemsAndSetNextStage()
    {
        foreach (string itemName in ItemsToDrop)
        {
            foreach (Item item in JSONLoader.JSONItems.ItemList)
            {
                if (item.ItemName == itemName)
                {
                    Vector2 randomizedPosition = Random.insideUnitCircle * Settings.DistanceRangeOfItemDropping;
                    GameObject droppedItem = Instantiate(Resources.Load<GameObject>("Prefabs/DroppedItem"), new Vector2(transform.position.x, transform.position.y) + randomizedPosition, Quaternion.identity);
                    droppedItem.GetComponent<GrabbingController>().CurrentItem = new Item(item.ItemID, item.ItemName, item.SpriteName, 1, item.Stackable, item.IsSeed, item.ItemValue, item.ItemDescription);
                    droppedItem.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Graphics/Sprites/{item.SpriteName}");
                }
            }
        }

        for (int x = 0; x < LoadSeedPercentages().GetLength(0); x++)
        {
            int randomizedNumber = Random.Range(0, 100);

            if (randomizedNumber < LoadSeedPercentages()[x, 1])
            {
                foreach (Item item in JSONLoader.JSONItems.ItemList)
                {
                    if (item.ItemID == LoadSeedPercentages()[x, 0])
                    {
                        Vector2 randomizedPosition = Random.insideUnitCircle * Settings.DistanceRangeOfItemDropping;
                        GameObject droppedItem = Instantiate(Resources.Load<GameObject>("Prefabs/DroppedItem"), new Vector2(transform.position.x, transform.position.y) + randomizedPosition, Quaternion.identity);
                        droppedItem.GetComponent<GrabbingController>().CurrentItem = new Item(item.ItemID, item.ItemName, item.SpriteName, 1, item.Stackable, item.IsSeed, item.ItemValue, item.ItemDescription);
                        droppedItem.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Graphics/Sprites/{item.SpriteName}");
                    }
                }
            }
        }

        transform.GetChild(0).gameObject.SetActive(false);

        SetNextStage();
    }

    public void AddTick()
    {
        Ticks += 1;
        if (Ticks >= TicksNeededForNextStage)
            SetNextStage();
    }

    public void SetNextStage()
    {
        switch (FieldStage)
        {
            case FarmStage.Empty:
                GetComponent<SpriteRenderer>().sprite = SeedsPlanted;
                FieldStage = FarmStage.SeedsPlanted;
                break;
            case FarmStage.SeedsPlanted:
                GetComponent<SpriteRenderer>().sprite = Growing;
                FieldStage = FarmStage.Growing;
                break;
            case FarmStage.Growing:
                GetComponent<SpriteRenderer>().sprite = ReadyToHarvest;
                FieldStage = FarmStage.ReadyToHarvest;
                break;
            case FarmStage.ReadyToHarvest:
                GetComponent<SpriteRenderer>().sprite = Empty;
                FieldStage = FarmStage.Empty;
                break;
        }
        
        Ticks = 0;
    }
}
