using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public List<GameObject> InventorySlots = new List<GameObject>(), HotbarSlots = new List<GameObject>();
    public InventoryType TypeOfInventory;
    public static GameObject GrabbedSlot = null, GrabbedSlotPrefab = null, MainInventoryObject, ActiveHotbarSlot;
    public static bool InventoryOpen = false;

    public enum InventoryType
    {
        PlayerInventory,
        InteractableObjectInventory
    }

    private void Update()
    {
        if (tag == "InventoryContainer")
            ManageInventoryVisibility();

        if (tag == "HotbarContainer")
            SwitchHotbarSlots();
    }

    private void OnEnable()
    {
        if (tag == "InventoryContainer")
        {
            MainInventoryObject = GameObject.FindGameObjectWithTag("InventoryContainer");
            InventoryOpen = MainInventoryObject.activeInHierarchy;
            CloseInventory();
        }
        if (InventorySlots.Count == 0)
            LoadAllSlots();
        if (tag == "HotbarContainer")
        {
            ActiveHotbarSlot = HotbarSlots[0];
            ActiveHotbarSlot.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    private void SwitchHotbarSlots()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActiveHotbarSlot.transform.GetChild(1).gameObject.SetActive(false);
            ActiveHotbarSlot = HotbarSlots[0];
            ActiveHotbarSlot.transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActiveHotbarSlot.transform.GetChild(1).gameObject.SetActive(false);
            ActiveHotbarSlot = HotbarSlots[1];
            ActiveHotbarSlot.transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ActiveHotbarSlot.transform.GetChild(1).gameObject.SetActive(false);
            ActiveHotbarSlot = HotbarSlots[2];
            ActiveHotbarSlot.transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ActiveHotbarSlot.transform.GetChild(1).gameObject.SetActive(false);
            ActiveHotbarSlot = HotbarSlots[3];
            ActiveHotbarSlot.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    private void LoadAllSlots()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "InventorySlot")
                InventorySlots.Add(child.gameObject);
        }
    }

    public int AddItem(string itemName, int quantity)
    {
        foreach (GameObject slot in InventorySlots)
        {
            if (slot.GetComponent<InventorySlotController>().SlotEmpty)
            {
                foreach (Item item in JSONLoader.JSONItems.ItemList)
                {
                    if (item.ItemName == itemName)
                    {
                        slot.GetComponent<InventorySlotController>().SlotItem = new Item(item.ItemID, item.ItemName, item.SpriteName, quantity <= Settings.MaxItemQuantityPerSlot ? quantity : Settings.MaxItemQuantityPerSlot, item.Stackable, item.IsSeed, item.ItemValue, item.ItemDescription);
                        slot.GetComponent<InventorySlotController>().UpdateGUI(Resources.Load<Sprite>($"Graphics/Sprites/{item.SpriteName}"));
                        slot.GetComponent<InventorySlotController>().SlotEmpty = false;

                        return quantity <= Settings.MaxItemQuantityPerSlot ? 0 : quantity - Settings.MaxItemQuantityPerSlot;
                    }
                }
            }
            else if (slot.GetComponent<InventorySlotController>().SlotItem.Stackable && slot.GetComponent<InventorySlotController>().SlotItem.Amount < Settings.MaxItemQuantityPerSlot && slot.GetComponent<InventorySlotController>().SlotItem.ItemName == itemName)
            {
                if (slot.GetComponent<InventorySlotController>().SlotItem.Amount + quantity <= Settings.MaxItemQuantityPerSlot)
                {
                    slot.GetComponent<InventorySlotController>().SlotItem.Amount += quantity;
                    slot.GetComponent<InventorySlotController>().UpdateAmount();

                    return 0;
                }
                else
                {
                    int originalSlotAmount = slot.GetComponent<InventorySlotController>().SlotItem.Amount;

                    slot.GetComponent<InventorySlotController>().SlotItem.Amount = Settings.MaxItemQuantityPerSlot;
                    slot.GetComponent<InventorySlotController>().UpdateAmount();

                    return quantity - (Settings.MaxItemQuantityPerSlot - originalSlotAmount);
                }
            }
        }
        return quantity;
    }

    public int AddItemToSlot(GameObject slot, string itemName, int quantity)
    {
        if (slot.GetComponent<InventorySlotController>().SlotEmpty)
        {
            foreach (Item item in JSONLoader.JSONItems.ItemList)
            {
                if (item.ItemName == itemName)
                {
                    slot.GetComponent<InventorySlotController>().SlotItem = new Item(item.ItemID, item.ItemName, item.SpriteName, quantity <= Settings.MaxItemQuantityPerSlot ? quantity : Settings.MaxItemQuantityPerSlot, item.Stackable, item.IsSeed, item.ItemValue, item.ItemDescription);
                    slot.GetComponent<InventorySlotController>().UpdateGUI(Resources.Load<Sprite>($"Graphics/Sprites/{item.SpriteName}"));
                    slot.GetComponent<InventorySlotController>().SlotEmpty = false;

                    return quantity <= Settings.MaxItemQuantityPerSlot ? 0 : quantity - Settings.MaxItemQuantityPerSlot;
                }
            }
        }
        else if (slot.GetComponent<InventorySlotController>().SlotItem.Stackable && slot.GetComponent<InventorySlotController>().SlotItem.Amount < Settings.MaxItemQuantityPerSlot && slot.GetComponent<InventorySlotController>().SlotItem.ItemName == itemName)
        {
            if (slot.GetComponent<InventorySlotController>().SlotItem.Amount + quantity <= Settings.MaxItemQuantityPerSlot)
            {
                slot.GetComponent<InventorySlotController>().SlotItem.Amount += quantity;
                slot.GetComponent<InventorySlotController>().UpdateAmount();

                return 0;
            }
            else
            {
                int originalSlotAmount = slot.GetComponent<InventorySlotController>().SlotItem.Amount;

                slot.GetComponent<InventorySlotController>().SlotItem.Amount = Settings.MaxItemQuantityPerSlot;
                slot.GetComponent<InventorySlotController>().UpdateAmount();

                return quantity - (Settings.MaxItemQuantityPerSlot - originalSlotAmount);
            }
        }
        return quantity;
    }

    public int RemoveItem(string itemName, int quantity)
    {
        foreach (GameObject slot in InventorySlots)
        {
            if (!slot.GetComponent<InventorySlotController>().SlotEmpty && slot.GetComponent<InventorySlotController>().SlotItem.ItemName == itemName)
            {
                if (slot.GetComponent<InventorySlotController>().SlotItem.Amount - quantity <= 0)
                {
                    int originalSlotAmount = slot.GetComponent<InventorySlotController>().SlotItem.Amount;
                    slot.GetComponent<InventorySlotController>().SlotItem = null;
                    slot.GetComponent<InventorySlotController>().SlotEmpty = true;
                    slot.GetComponent<InventorySlotController>().EmptyGUI();
                    return quantity - originalSlotAmount;
                }
                else
                {
                    slot.GetComponent<InventorySlotController>().SlotItem.Amount -= quantity;
                    slot.GetComponent<InventorySlotController>().UpdateAmount();
                    return 0;
                }
            }
        }
        return quantity;
    }

    public static int RemoveItemQuantityFromSlot(GameObject slot, int quantity)
    {
        if (slot.GetComponent<InventorySlotController>().SlotItem.Amount - quantity <= 0)
        {
            int originalSlotAmount = slot.GetComponent<InventorySlotController>().SlotItem.Amount;
            slot.GetComponent<InventorySlotController>().SlotItem = null;
            slot.GetComponent<InventorySlotController>().SlotEmpty = true;
            slot.GetComponent<InventorySlotController>().EmptyGUI();

            GrabbingController.EditItemList(GameObject.FindGameObjectWithTag("InventoryContainer"), GameObject.FindGameObjectWithTag("HotbarContainer"));

            return quantity - originalSlotAmount;
        }
        else
        {
            slot.GetComponent<InventorySlotController>().SlotItem.Amount -= quantity;
            slot.GetComponent<InventorySlotController>().UpdateAmount();

            GrabbingController.EditItemList(GameObject.FindGameObjectWithTag("InventoryContainer"), GameObject.FindGameObjectWithTag("HotbarContainer"));

            return 0;
        }
    }

    public static void ManageInventoryVisibility()
    {
        if (Input.GetKeyDown(Settings.InventoryButton))
        {
            if (InventoryOpen)
                CloseInventory();
            else
                OpenInventory();
        }
    }

    public static void CloseInventory()
    {
        MainInventoryObject.transform.localPosition = Settings.ClosedInventoryPosition;
        InventoryOpen = false;

        if (GrabbedSlot != null)
        {
            Item grabbedSlotItem = GrabbedSlotPrefab.GetComponent<InventorySlotController>().SlotItem;

            int leftAmount = GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<ItemManagementController>().AddItemToInventory(grabbedSlotItem.ItemName, grabbedSlotItem.Amount);
            if (leftAmount > 0)
            {
                GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<ItemDropper>().DropItem(grabbedSlotItem.ItemName, leftAmount);
            }

            Destroy(GrabbedSlotPrefab);
            GrabbedSlotPrefab = null;
            GrabbedSlot = null;
        }
    }

    public static void OpenInventory()
    {
        SetInventoryPosition();
        InventoryOpen = true;
    }

    public static void SetInventoryPosition()
    {
        if (Settings.InteractableInventoryType != Settings.OpenInteractableInventoryType.Empty)
        {
            MainInventoryObject.transform.localPosition = Settings.OpenInventoryPositionWithChestOpen;
        }
        else
        {
            MainInventoryObject.transform.localPosition = Settings.OpenInventoryPosition;
        }
    }
}
