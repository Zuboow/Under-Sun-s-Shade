using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotController : MonoBehaviour
{
    public Item SlotItem;
    public bool SlotEmpty = true, Clickable = false, HotbarSlot = false, InventorySlot = false, FurnaceInputSlot = false, FurnaceFuelSlot = false, FurnaceOutputSlot = false;

    private void OnEnable()
    {
        SlotItem = null;
    }

    private void Update()
    {
        if (InventoryController.GrabbedSlotPrefab != null)
            MoveGrabbedItemToMousePosition();
    }

    private static void MoveGrabbedItemToMousePosition()
    {
        InventoryController.GrabbedSlotPrefab.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 10f);
    }

    public void CheckRaycastHit()
    {
        if (InventoryController.InventoryOpen)
        {
            if (Input.GetKeyDown(Settings.DroppingButton))
            {
                DropItemFromInventory(gameObject);
                GrabbingController.EditItemList(GameObject.FindGameObjectWithTag("InventoryContainer"), GameObject.FindGameObjectWithTag("HotbarContainer"));
                UpdateOpenInteractableObjectInventory();
            }
            else if (Input.GetMouseButtonDown(0) && Input.GetKey(Settings.AutoAddButton) && ChestController.CurrentChest == null && Settings.InteractableInventoryType == Settings.OpenInteractableInventoryType.Empty && !SlotEmpty && Clickable)
            {
                AutoMoveBetweenInventoryAndHotbar(gameObject, HotbarSlot, SlotItem.ItemName, SlotItem.Amount);
                GrabbingController.EditItemList(GameObject.FindGameObjectWithTag("InventoryContainer"), GameObject.FindGameObjectWithTag("HotbarContainer"));
            }
            else if (Input.GetMouseButtonDown(0) && Input.GetKey(Settings.AutoAddButton) && Settings.InteractableInventoryType == Settings.OpenInteractableInventoryType.Furnace && !SlotEmpty)
            {
                AutoMoveBetweenFurnaceAndInventory(gameObject, InventorySlot, HotbarSlot, SlotItem.ItemName, SlotItem.Amount, FurnaceInputSlot, FurnaceFuelSlot, FurnaceOutputSlot);
                GrabbingController.EditItemList(GameObject.FindGameObjectWithTag("InventoryContainer"), GameObject.FindGameObjectWithTag("HotbarContainer"));
            }
            else if (Input.GetMouseButtonDown(0) && Input.GetKey(Settings.AutoAddButton) && ChestController.CurrentChest != null && !SlotEmpty && Clickable)
            {
                AutoMoveToInventory(gameObject, InventorySlot, HotbarSlot, SlotItem.ItemName, SlotItem.Amount);
                GrabbingController.EditItemList(GameObject.FindGameObjectWithTag("InventoryContainer"), GameObject.FindGameObjectWithTag("HotbarContainer"));
                UpdateOpenInteractableObjectInventory();
            }
            else if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && InventoryController.GrabbedSlot != null && Clickable)
            {
                DecideItemReplacement(gameObject);
                GrabbingController.EditItemList(GameObject.FindGameObjectWithTag("InventoryContainer"), GameObject.FindGameObjectWithTag("HotbarContainer"));
                UpdateOpenInteractableObjectInventory();
            }
            else if (Input.GetMouseButtonDown(1) && InventoryController.GrabbedSlot == null && !SlotEmpty)
            {
                TakeHalf(gameObject);
                GrabbingController.EditItemList(GameObject.FindGameObjectWithTag("InventoryContainer"), GameObject.FindGameObjectWithTag("HotbarContainer"));
                UpdateOpenInteractableObjectInventory();
            }
            else if (Input.GetMouseButtonDown(0) && InventoryController.GrabbedSlot == null && !SlotEmpty)
            {
                TakeAll(gameObject);
                GrabbingController.EditItemList(GameObject.FindGameObjectWithTag("InventoryContainer"), GameObject.FindGameObjectWithTag("HotbarContainer"));
                UpdateOpenInteractableObjectInventory();
            }
        }
    }

    public static void AutoMoveToInventory(GameObject clickedSlot, bool inventorySlot, bool hotbarSlot, string itemName, int quantity)
    {
        int leftAmount = quantity;
        if (inventorySlot)
            leftAmount = GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<ItemManagementController>().AddItemToChest(itemName, leftAmount);
        else
            leftAmount = GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<ItemManagementController>().AddItemToInventory(itemName, leftAmount);

        if (leftAmount != 0)
            AutoMoveBetweenInventoryAndHotbar(clickedSlot, hotbarSlot, itemName, quantity);

        if (leftAmount == 0)
        {
            clickedSlot.GetComponent<InventorySlotController>().SlotItem = null;
            clickedSlot.GetComponent<InventorySlotController>().SlotEmpty = true;
            clickedSlot.GetComponent<InventorySlotController>().EmptyGUI();
        }
        else if (leftAmount != quantity)
        {
            clickedSlot.GetComponent<InventorySlotController>().SlotItem.Amount = leftAmount;
            clickedSlot.GetComponent<InventorySlotController>().UpdateAmount();
        }
    }

    public static void AutoMoveBetweenFurnaceAndInventory(GameObject clickedSlot, bool inventorySlot, bool hotbarSlot, string itemName, int quantity, bool inputSlot, bool fuelSlot, bool outputSlot)
    {
        int leftAmount = quantity;
        if (!inventorySlot)
        {
            leftAmount = GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<ItemManagementController>().AddItemToInventory(itemName, leftAmount);

            if (inputSlot)
                FurnaceController.CurrentFurnace.GetComponent<FurnaceController>().RemoveItemFromInput(leftAmount);
            if (outputSlot)
                FurnaceController.CurrentFurnace.GetComponent<FurnaceController>().RemoveItemFromOutput(leftAmount);
            if (fuelSlot)
                FurnaceController.CurrentFurnace.GetComponent<FurnaceController>().RemoveItemFromFuel(leftAmount);
        }
        else
        {
            if (Settings.SmeltingInputAndOutput.ContainsKey(itemName))
                leftAmount = FurnaceController.CurrentFurnace.GetComponent<FurnaceController>().AddItemToInput(itemName, quantity);
            else if (Settings.FuelNameAndTime.ContainsKey(itemName))
                leftAmount = FurnaceController.CurrentFurnace.GetComponent<FurnaceController>().AddItemToFuel(itemName, quantity);

            if (leftAmount == 0)
            {
                clickedSlot.GetComponent<InventorySlotController>().SlotItem = null;
                clickedSlot.GetComponent<InventorySlotController>().SlotEmpty = true;
                clickedSlot.GetComponent<InventorySlotController>().EmptyGUI();
            }
            else if (leftAmount != quantity)
            {
                clickedSlot.GetComponent<InventorySlotController>().SlotItem.Amount = leftAmount;
                clickedSlot.GetComponent<InventorySlotController>().UpdateAmount();
            }
        }
    }

    public static void AutoMoveBetweenInventoryAndHotbar(GameObject clickedSlot, bool inventorySlot, string itemName, int quantity)
    {
        int leftAmount = GameObject.FindGameObjectWithTag(!inventorySlot ? "HotbarContainer" : "InventoryContainer").GetComponent<InventoryController>().AddItem(itemName, quantity);

        if (leftAmount == 0)
        {
            clickedSlot.GetComponent<InventorySlotController>().SlotItem = null;
            clickedSlot.GetComponent<InventorySlotController>().SlotEmpty = true;
            clickedSlot.GetComponent<InventorySlotController>().EmptyGUI();
        }
        else if (leftAmount != quantity)
        {
            clickedSlot.GetComponent<InventorySlotController>().SlotItem.Amount = leftAmount;
            clickedSlot.GetComponent<InventorySlotController>().UpdateAmount();
        }
    }

    public static void UpdateOpenInteractableObjectInventory()
    {
        switch (Settings.InteractableInventoryType)
        {
            case Settings.OpenInteractableInventoryType.Chest:
                UpdateCurrentChest();
                break;
            case Settings.OpenInteractableInventoryType.Furnace:
                UpdateCurrentFurnace();
                break;
        }
    }

    public static void UpdateCurrentChest()
    {
        if (ChestController.CurrentChest != null)
        {
            List<GameObject> chestSlots = ChestController.ChestUI.activeInHierarchy ? ChestController.ChestUI.GetComponent<InventoryController>().InventorySlots : ChestController.TradingChestUI.GetComponent<InventoryController>().InventorySlots;
            List<Item> listOfChestItems = ChestController.CurrentChest.GetComponent<ChestController>().Contents;
            for (int x = 0; x < (ChestController.ChestUI.activeInHierarchy ? 9 : 6); x++)
            {
                if (chestSlots[x].GetComponent<InventorySlotController>().SlotItem != null)
                {
                    listOfChestItems[x] = chestSlots[x].GetComponent<InventorySlotController>().SlotItem;
                }
                else
                {
                    listOfChestItems[x] = null;
                }
            }
        }
    }

    public static void UpdateCurrentFurnace()
    {
        if (FurnaceController.CurrentFurnace != null)
        {
            List<GameObject> FurnaceSlots = FurnaceController.FurnaceUI.GetComponent<InventoryController>().InventorySlots;
            FurnaceController.CurrentFurnace.GetComponent<FurnaceController>().InputItem = FurnaceSlots[0].GetComponent<InventorySlotController>().SlotItem == null ? null : FurnaceSlots[0].GetComponent<InventorySlotController>().SlotItem;
            FurnaceController.CurrentFurnace.GetComponent<FurnaceController>().OutputItem = FurnaceSlots[1].GetComponent<InventorySlotController>().SlotItem == null ? null : FurnaceSlots[1].GetComponent<InventorySlotController>().SlotItem;
            FurnaceController.CurrentFurnace.GetComponent<FurnaceController>().FuelItem = FurnaceSlots[2].GetComponent<InventorySlotController>().SlotItem == null ? null : FurnaceSlots[2].GetComponent<InventorySlotController>().SlotItem;
        }
    }

    public static void DropItemFromInventory(GameObject clickedSlot)
    {
        if (clickedSlot.GetComponent<InventorySlotController>().SlotItem != null)
        {
            Item clickedSlotItem = clickedSlot.GetComponent<InventorySlotController>().SlotItem;
            GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<ItemDropper>().DropItem(clickedSlotItem.ItemName, clickedSlotItem.Amount);

            clickedSlot.GetComponent<InventorySlotController>().SlotItem = null;
            clickedSlot.GetComponent<InventorySlotController>().SlotEmpty = true;
            clickedSlot.GetComponent<InventorySlotController>().EmptyGUI();
        }
    }

    public static void DecideItemReplacement(GameObject clickedSlot)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (clickedSlot.GetComponent<InventorySlotController>().SlotEmpty || !clickedSlot.GetComponent<InventorySlotController>().SlotItem.Stackable ||
                clickedSlot.GetComponent<InventorySlotController>().SlotItem.ItemID != InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem.ItemID ||
                (clickedSlot.GetComponent<InventorySlotController>().SlotItem.ItemID == InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem.ItemID &&
                clickedSlot.GetComponent<InventorySlotController>().SlotItem.Amount == Settings.MaxItemQuantityPerSlot))
            {
                ReplaceHeldItem(clickedSlot);
            }
            else
            {
                MoveQuantities(clickedSlot);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (!clickedSlot.GetComponent<InventorySlotController>().SlotEmpty &&
                (clickedSlot.GetComponent<InventorySlotController>().SlotItem.ItemID != InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem.ItemID ||
                (clickedSlot.GetComponent<InventorySlotController>().SlotItem.ItemID == InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem.ItemID &&
                clickedSlot.GetComponent<InventorySlotController>().SlotItem.Amount == Settings.MaxItemQuantityPerSlot) || !clickedSlot.GetComponent<InventorySlotController>().SlotItem.Stackable))
            {
                ReplaceHeldItem(clickedSlot);
            }
            else if (clickedSlot.GetComponent<InventorySlotController>().SlotEmpty)
            {
                PlaceOneToEmptySlot(clickedSlot);
            }
            else
            {
                MoveOne(clickedSlot);
            }
        }
    }

    public static void TakeAll(GameObject grabbedSlot)
    {
        InventoryController.GrabbedSlot = grabbedSlot;
        InventoryController.GrabbedSlotPrefab = Instantiate(Resources.Load<GameObject>("Prefabs/GrabbedObjectBlueprint"), InventoryController.GrabbedSlot.transform.position, Quaternion.identity);
        InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem = InventoryController.GrabbedSlot.GetComponent<InventorySlotController>().SlotItem;
        InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().UpdateGrabbedItemGUI(Resources.Load<Sprite>($"Graphics/Sprites/{InventoryController.GrabbedSlot.GetComponent<InventorySlotController>().SlotItem.SpriteName}"));

        InventoryController.GrabbedSlot.GetComponent<InventorySlotController>().SlotItem = null;
        InventoryController.GrabbedSlot.GetComponent<InventorySlotController>().SlotEmpty = true;
        InventoryController.GrabbedSlot.GetComponent<InventorySlotController>().EmptyGUI();
    }

    public static void TakeHalf(GameObject grabbedSlot)
    {
        InventoryController.GrabbedSlot = grabbedSlot;
        Item grabbedSlotItem = InventoryController.GrabbedSlot.GetComponent<InventorySlotController>().SlotItem;

        float takenAmount = Mathf.Ceil(InventoryController.GrabbedSlot.GetComponent<InventorySlotController>().SlotItem.Amount * 0.5f);
        float leftAmount = Mathf.Floor(InventoryController.GrabbedSlot.GetComponent<InventorySlotController>().SlotItem.Amount - takenAmount);

        InventoryController.GrabbedSlotPrefab = Instantiate(Resources.Load<GameObject>("Prefabs/GrabbedObjectBlueprint"), InventoryController.GrabbedSlot.transform.position, Quaternion.identity);
        InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem = new Item(grabbedSlotItem.ItemID, grabbedSlotItem.ItemName, grabbedSlotItem.SpriteName, (int)takenAmount, grabbedSlotItem.Stackable, grabbedSlotItem.IsSeed, grabbedSlotItem.ItemValue, grabbedSlotItem.ItemDescription);

        if (InventoryController.GrabbedSlot.GetComponent<InventorySlotController>().SlotItem.Amount > 1)
        {
            InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem.Amount = (int)takenAmount;
            InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().UpdateGrabbedItemGUI(Resources.Load<Sprite>($"Graphics/Sprites/{InventoryController.GrabbedSlot.GetComponent<InventorySlotController>().SlotItem.SpriteName}"));
            InventoryController.GrabbedSlot.GetComponent<InventorySlotController>().SlotItem.Amount = (int)leftAmount;
            InventoryController.GrabbedSlot.GetComponent<InventorySlotController>().UpdateAmount();
        }
        else
        {
            InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem.Amount = (int)takenAmount;
            InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().UpdateGrabbedItemGUI(Resources.Load<Sprite>($"Graphics/Sprites/{InventoryController.GrabbedSlot.GetComponent<InventorySlotController>().SlotItem.SpriteName}"));
            InventoryController.GrabbedSlot.GetComponent<InventorySlotController>().SlotItem = null;
            InventoryController.GrabbedSlot.GetComponent<InventorySlotController>().SlotEmpty = true;
            InventoryController.GrabbedSlot.GetComponent<InventorySlotController>().EmptyGUI();
        }
    }

    public static void PlaceOneToEmptySlot(GameObject clickedSlot)
    {
        Item grabbedSlotItem = InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem;

        clickedSlot.GetComponent<InventorySlotController>().SlotItem = new Item(grabbedSlotItem.ItemID, grabbedSlotItem.ItemName, grabbedSlotItem.SpriteName, 1, grabbedSlotItem.Stackable, grabbedSlotItem.IsSeed, grabbedSlotItem.ItemValue, grabbedSlotItem.ItemDescription);
        clickedSlot.GetComponent<InventorySlotController>().UpdateGUI(Resources.Load<Sprite>($"Graphics/Sprites/{grabbedSlotItem.SpriteName}"));
        clickedSlot.GetComponent<InventorySlotController>().SlotEmpty = false;

        if (InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem.Amount == 1)
        {
            Destroy(InventoryController.GrabbedSlotPrefab);
            InventoryController.GrabbedSlotPrefab = null;
            InventoryController.GrabbedSlot = null;
        }
        else
        {
            InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem.Amount -= 1;
            InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().UpdateAmount();
        }
    }

    public static void MoveOne(GameObject clickedSlot)
    {
        Item grabbedSlotItem = InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem;
        Item clickedSlotItem = clickedSlot.GetComponent<InventorySlotController>().SlotItem;

        if (clickedSlotItem.Amount + 1 > Settings.MaxItemQuantityPerSlot)
        {
            InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem.Amount = grabbedSlotItem.Amount - (Settings.MaxItemQuantityPerSlot - clickedSlotItem.Amount);
            clickedSlot.GetComponent<InventorySlotController>().SlotItem.Amount = Settings.MaxItemQuantityPerSlot;

            clickedSlot.GetComponent<InventorySlotController>().UpdateAmount();
            InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().UpdateAmount();
        }
        else
        {
            clickedSlot.GetComponent<InventorySlotController>().SlotItem.Amount += 1;
            clickedSlot.GetComponent<InventorySlotController>().UpdateAmount();

            if (InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem.Amount == 1)
            {
                Destroy(InventoryController.GrabbedSlotPrefab);
                InventoryController.GrabbedSlotPrefab = null;
                InventoryController.GrabbedSlot = null;
            }
            else
            {
                InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem.Amount -= 1;
                InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().UpdateAmount();
            }
        }
    }

    public static void MoveQuantities(GameObject clickedSlot)
    {
        Item grabbedSlotItem = InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem;
        Item clickedSlotItem = clickedSlot.GetComponent<InventorySlotController>().SlotItem;

        if (clickedSlotItem.Amount + grabbedSlotItem.Amount > Settings.MaxItemQuantityPerSlot)
        {
            InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem.Amount = grabbedSlotItem.Amount - (Settings.MaxItemQuantityPerSlot - clickedSlotItem.Amount);
            clickedSlot.GetComponent<InventorySlotController>().SlotItem.Amount = Settings.MaxItemQuantityPerSlot;

            clickedSlot.GetComponent<InventorySlotController>().UpdateAmount();
            InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().UpdateAmount();
        }
        else
        {
            clickedSlot.GetComponent<InventorySlotController>().SlotItem.Amount += grabbedSlotItem.Amount;
            clickedSlot.GetComponent<InventorySlotController>().UpdateAmount();

            Destroy(InventoryController.GrabbedSlotPrefab);
            InventoryController.GrabbedSlotPrefab = null;
            InventoryController.GrabbedSlot = null;
        }
    }

    public static void ReplaceHeldItem(GameObject clickedSlot)
    {
        Item grabbedSlotItem = InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem;
        Item clickedSlotItem = clickedSlot.GetComponent<InventorySlotController>().SlotItem;
        bool clickedSlotEmpty = false;

        if (clickedSlotItem != null)
        {
            InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem = clickedSlotItem;
            InventoryController.GrabbedSlotPrefab.GetComponent<InventorySlotController>().UpdateGrabbedItemGUI(Resources.Load<Sprite>($"Graphics/Sprites/{clickedSlot.GetComponent<InventorySlotController>().SlotItem.SpriteName}"));
        }
        else
        {
            clickedSlotEmpty = true;
        }
        clickedSlot.GetComponent<InventorySlotController>().SlotItem = grabbedSlotItem;
        clickedSlot.GetComponent<InventorySlotController>().SlotEmpty = false;

        clickedSlot.GetComponent<InventorySlotController>().UpdateGUI(Resources.Load<Sprite>($"Graphics/Sprites/{grabbedSlotItem.SpriteName}"));

        if (clickedSlotEmpty)
        {
            Destroy(InventoryController.GrabbedSlotPrefab);
            InventoryController.GrabbedSlotPrefab = null;
            InventoryController.GrabbedSlot = null;
        }
    }

    public void UpdateGUI(Sprite newSprite)
    {
        GetComponent<Image>().sprite = newSprite;
        transform.GetChild(0).GetComponent<TextMeshPro>().text = SlotItem.Stackable ? SlotItem.Amount.ToString() : "";
    }

    public void UpdateGrabbedItemGUI(Sprite newSprite)
    {
        GetComponent<SpriteRenderer>().sprite = newSprite;
        transform.GetChild(0).GetComponent<TextMeshPro>().text = SlotItem.Stackable ? SlotItem.Amount.ToString() : "";
    }

    public void EmptyGUI()
    {
        GetComponent<Image>().sprite = Resources.Load<Sprite>($"Graphics/Sprites/Empty");
        transform.GetChild(0).GetComponent<TextMeshPro>().text = "";
    }

    public void UpdateAmount()
    {
        transform.GetChild(0).GetComponent<TextMeshPro>().text = SlotItem.Stackable ? SlotItem.Amount.ToString() : "";
    }

}
