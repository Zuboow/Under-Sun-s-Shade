using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public List<Item> Contents = new List<Item>();
    public static GameObject ChestUI, TradingChestUI, CurrentChest;
    public bool TradingChest, Interactable;

    private void OnEnable()
    {
        if (ChestUI == null && !TradingChest)
        {
            ChestUI = GameObject.FindGameObjectWithTag("ChestInventory"); 
            ChestUI.SetActive(false);
        }
        if (TradingChestUI == null && TradingChest)
        {
            TradingChestUI = GameObject.FindGameObjectWithTag("TradingChestInventory");
            TradingChestUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(Settings.InventoryButton) && CurrentChest != null)
        {
            RemoveItemsFromSlots();
        }

        if (Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) > Settings.DistanceForObjectsToBeInteractable && transform.GetChild(0).gameObject.activeInHierarchy)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void DecideOnMouseOver()
    {
        if (Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) < Settings.DistanceForObjectsToBeInteractable && !InventoryController.InventoryOpen)
        {
            if (Input.GetKeyDown(KeyCode.F) && CurrentChest == null && Interactable)
            {
                GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<AudioPlayer>().PlaySound("chestOpening");
                InsertItemsIntoSlots();
                if (!InventoryController.InventoryOpen)
                    InventoryController.OpenInventory();
                else
                {
                    InventoryController.SetInventoryPosition();
                }
            }
            if (CurrentChest == null && Interactable)
                transform.GetChild(0).gameObject.SetActive(true);
            else
                transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void OnMouseExit()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void InsertItemsIntoSlots()
    {
        if (!TradingChest)
            ChestUI.SetActive(true);
        else
            TradingChestUI.SetActive(true);
        CurrentChest = gameObject;
        Settings.InteractableInventoryType = Settings.OpenInteractableInventoryType.Chest;

        for (int x = 0; x < (!TradingChest ? 9 : 6); x++)
        {
            foreach (Item JSONitem in JSONLoader.JSONItems.ItemList)
            {
                if (Contents != null && Contents[x] != null && Contents[x].ItemName.Trim() != "" && JSONitem.ItemID == Contents[x].ItemID)
                {
                    GameObject selectedSlot = !TradingChest ? ChestUI.GetComponent<InventoryController>().InventorySlots[x] : TradingChestUI.GetComponent<InventoryController>().InventorySlots[x];
                    selectedSlot.GetComponent<InventorySlotController>().SlotItem = new Item(Contents[x].ItemID, Contents[x].ItemName, Contents[x].SpriteName, Contents[x].Amount, Contents[x].Stackable, Contents[x].IsSeed, Contents[x].ItemValue, Contents[x].ItemDescription);
                    selectedSlot.GetComponent<InventorySlotController>().UpdateGUI(Resources.Load<Sprite>($"Graphics/Sprites/{Contents[x].SpriteName}"));
                    selectedSlot.GetComponent<InventorySlotController>().SlotEmpty = false;
                }
            }
        }
    }

    public static void RemoveItemsFromSlots()
    {
        for (int x = 0; x < (ChestUI.activeInHierarchy ? 9 : 6); x++)
        {
            GameObject selectedSlot = ChestUI.activeInHierarchy ? ChestUI.GetComponent<InventoryController>().InventorySlots[x] : TradingChestUI.GetComponent<InventoryController>().InventorySlots[x];
            selectedSlot.GetComponent<InventorySlotController>().SlotItem = null;
            selectedSlot.GetComponent<InventorySlotController>().SlotEmpty = true;
            selectedSlot.GetComponent<InventorySlotController>().EmptyGUI();
        }
        if (ChestUI.activeInHierarchy)
            ChestUI.SetActive(false);
        if (TradingChestUI.activeInHierarchy)
            TradingChestUI.SetActive(false);
        CurrentChest = null;
        Settings.InteractableInventoryType = Settings.OpenInteractableInventoryType.Empty;
    }
}
