using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManagementController : MonoBehaviour
{
    public GameObject MainInventory, Hotbar, ChestInventory, TradingChestInventory;
    int CurrentItemAmount = 0;

    private void OnEnable()
    {
        GrabbingController.EditItemList(MainInventory, Hotbar);
    }

    void Update()
    {
        
    }

    public int AddItemToInventory(string itemName, int quantity)
    {
        CurrentItemAmount = quantity;

        foreach (GameObject slot in MainInventory.GetComponent<InventoryController>().InventorySlots)
        {
            if (slot.GetComponent<InventorySlotController>().SlotItem != null && slot.GetComponent<InventorySlotController>().SlotItem.Amount != Settings.MaxItemQuantityPerSlot && slot.GetComponent<InventorySlotController>().SlotItem.Stackable && CurrentItemAmount > 0 && itemName == slot.GetComponent<InventorySlotController>().SlotItem.ItemName)
            {
                int originalAmount = slot.GetComponent<InventorySlotController>().SlotItem.Amount;
                if (slot.GetComponent<InventorySlotController>().SlotItem.Amount + CurrentItemAmount > Settings.MaxItemQuantityPerSlot)
                    slot.GetComponent<InventorySlotController>().SlotItem.Amount = Settings.MaxItemQuantityPerSlot;
                else
                    slot.GetComponent<InventorySlotController>().SlotItem.Amount += CurrentItemAmount;
                CurrentItemAmount -= slot.GetComponent<InventorySlotController>().SlotItem.Amount - originalAmount;
                slot.GetComponent<InventorySlotController>().UpdateAmount();
            }
        }

        if (CurrentItemAmount != 0)
        {
            foreach (GameObject slot in Hotbar.GetComponent<InventoryController>().InventorySlots)
            {
                if (slot.GetComponent<InventorySlotController>().SlotItem != null && slot.GetComponent<InventorySlotController>().SlotItem.Amount != Settings.MaxItemQuantityPerSlot && slot.GetComponent<InventorySlotController>().SlotItem.Stackable && CurrentItemAmount > 0 && itemName == slot.GetComponent<InventorySlotController>().SlotItem.ItemName)
                {
                    int originalAmount = slot.GetComponent<InventorySlotController>().SlotItem.Amount;
                    if (slot.GetComponent<InventorySlotController>().SlotItem.Amount + CurrentItemAmount > Settings.MaxItemQuantityPerSlot)
                        slot.GetComponent<InventorySlotController>().SlotItem.Amount = Settings.MaxItemQuantityPerSlot;
                    else
                        slot.GetComponent<InventorySlotController>().SlotItem.Amount += CurrentItemAmount;
                    CurrentItemAmount -= slot.GetComponent<InventorySlotController>().SlotItem.Amount - originalAmount;
                    slot.GetComponent<InventorySlotController>().UpdateAmount();
                }
            }
        }

        if (CurrentItemAmount != 0)
        {
            for (int i = 0; i < Settings.MaxItemManagementTimes; i++)
            {
                CurrentItemAmount = MainInventory.GetComponent<InventoryController>().AddItem(itemName, CurrentItemAmount);
                if (CurrentItemAmount == 0)
                {
                    i = Settings.MaxItemManagementTimes - 1;
                }
                else if (CurrentItemAmount == quantity)
                {
                    i = Settings.MaxItemManagementTimes - 1;
                }
            }
        }

        if (CurrentItemAmount != 0)
        {
            for (int i = 0; i < Settings.MaxItemManagementTimes; i++)
            {
                CurrentItemAmount = Hotbar.GetComponent<InventoryController>().AddItem(itemName, CurrentItemAmount);
                if (CurrentItemAmount == 0)
                {
                    i = Settings.MaxItemManagementTimes - 1;
                }
                else if (CurrentItemAmount == quantity)
                {
                    i = Settings.MaxItemManagementTimes - 1;
                }
            }
        }

        GrabbingController.EditItemList(GameObject.FindGameObjectWithTag("InventoryContainer"), GameObject.FindGameObjectWithTag("HotbarContainer"));

        return CurrentItemAmount;
    }

    public int AddItemToChest(string itemName, int quantity)
    {
        CurrentItemAmount = quantity;

        foreach (GameObject slot in ChestController.ChestUI.activeInHierarchy ? ChestInventory.GetComponent<InventoryController>().InventorySlots : TradingChestInventory.GetComponent<InventoryController>().InventorySlots)
        {
            if (slot.GetComponent<InventorySlotController>().SlotItem != null && slot.GetComponent<InventorySlotController>().SlotItem.Amount != Settings.MaxItemQuantityPerSlot && CurrentItemAmount > 0 && itemName == slot.GetComponent<InventorySlotController>().SlotItem.ItemName && slot.GetComponent<InventorySlotController>().SlotItem.Stackable)
            {
                int originalAmount = slot.GetComponent<InventorySlotController>().SlotItem.Amount;
                if (slot.GetComponent<InventorySlotController>().SlotItem.Amount + CurrentItemAmount > Settings.MaxItemQuantityPerSlot)
                    slot.GetComponent<InventorySlotController>().SlotItem.Amount = Settings.MaxItemQuantityPerSlot;
                else
                    slot.GetComponent<InventorySlotController>().SlotItem.Amount += CurrentItemAmount;
                CurrentItemAmount -= slot.GetComponent<InventorySlotController>().SlotItem.Amount - originalAmount;
                slot.GetComponent<InventorySlotController>().UpdateAmount();
            }
        }
        if (CurrentItemAmount != 0)
        {
            for (int i = 0; i < Settings.MaxItemManagementTimes; i++)
            {
                CurrentItemAmount = ChestController.ChestUI.activeInHierarchy ? ChestInventory.GetComponent<InventoryController>().AddItem(itemName, CurrentItemAmount) : TradingChestInventory.GetComponent<InventoryController>().AddItem(itemName, CurrentItemAmount);
                if (CurrentItemAmount == 0)
                {
                    i = Settings.MaxItemManagementTimes - 1;
                }
                else if (CurrentItemAmount == quantity)
                {
                    i = Settings.MaxItemManagementTimes - 1;
                }
            }
        }

        return CurrentItemAmount;
    }

    public bool CheckIfItemQuantityIsPresent(string itemName, int quantity)
    {
        int leftQuantity = quantity;

        foreach (GameObject slot in MainInventory.GetComponent<InventoryController>().InventorySlots)
        {
            if (slot.GetComponent<InventorySlotController>().SlotItem != null && itemName == slot.GetComponent<InventorySlotController>().SlotItem.ItemName)
            {
                leftQuantity -= slot.GetComponent<InventorySlotController>().SlotItem.Amount;
            }
        }

        if (leftQuantity > 0)
        {
            foreach (GameObject slot in Hotbar.GetComponent<InventoryController>().InventorySlots)
            {
                if (slot.GetComponent<InventorySlotController>().SlotItem != null && itemName == slot.GetComponent<InventorySlotController>().SlotItem.ItemName)
                {
                    leftQuantity -= slot.GetComponent<InventorySlotController>().SlotItem.Amount;
                }
            }
        }

        if (leftQuantity <= 0)
            return true;
        else
            return false;
    }

    public int RemoveItemFromInventory(string itemName, int quantity)
    {
        CurrentItemAmount = quantity;

        for (int i = 0; i < Settings.MaxItemManagementTimes; i++)
        {
            CurrentItemAmount = MainInventory.GetComponent<InventoryController>().RemoveItem(itemName, CurrentItemAmount);
            if (CurrentItemAmount == 0)
            {
                i = Settings.MaxItemManagementTimes - 1;
            }
            else if (CurrentItemAmount == quantity)
            {
                i = Settings.MaxItemManagementTimes - 1;
            }
        }

        if (CurrentItemAmount != 0)
        {
            for (int i = 0; i < Settings.MaxItemManagementTimes; i++)
            {
                CurrentItemAmount = Hotbar.GetComponent<InventoryController>().RemoveItem(itemName, CurrentItemAmount);
                if (CurrentItemAmount == 0)
                {
                    i = Settings.MaxItemManagementTimes - 1;
                }
                else if (CurrentItemAmount == quantity)
                {
                    i = Settings.MaxItemManagementTimes - 1;
                }
            }
        }

        return CurrentItemAmount;
    }
}
