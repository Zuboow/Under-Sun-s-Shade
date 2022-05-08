using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnaceController : MonoBehaviour
{
    public Item InputItem = null, OutputItem = null, FuelItem = null;
    public static GameObject FurnaceUI, CurrentFurnace;
    public bool FurnaceOpen;
    public Animator FurnaceAnimator;

    private float TimeToSmelt = 3f, CurrentSmeltingTime = 0f, CurrentFuelTime = 0f, CurrentFuelItemDuration = 0f;

    private void OnEnable()
    {
        if (FurnaceUI == null)
        {
            FurnaceUI = GameObject.FindGameObjectWithTag("FurnaceInventory");
            FurnaceUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(Settings.InventoryButton) && /*Settings.InteractableInventoryType != Settings.OpenInteractableInventoryType.Empty &&*/ FurnaceOpen)
        {
            CloseUI();
            FurnaceOpen = false;
        }

        if (Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) > Settings.DistanceForObjectsToBeInteractable && transform.GetChild(0).gameObject.activeInHierarchy)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        CheckFurnaceStatus();
    }

    public void DecideOnMouseOver()
    {
        if (Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) < Settings.DistanceForObjectsToBeInteractable && !InventoryController.InventoryOpen)
        {
            if (Input.GetKeyDown(KeyCode.F) && Settings.InteractableInventoryType == Settings.OpenInteractableInventoryType.Empty)
            {
                GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<AudioPlayer>().PlaySound("chestOpening");
                OpenUI();
                if (!InventoryController.InventoryOpen)
                    InventoryController.OpenInventory();
                else
                {
                    InventoryController.SetInventoryPosition();
                }
            }
            if (Settings.InteractableInventoryType == Settings.OpenInteractableInventoryType.Empty)
                transform.GetChild(0).gameObject.SetActive(true);
            else
                transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void OnMouseExit()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void OpenUI()
    {
        FurnaceUI.SetActive(true);
        Settings.InteractableInventoryType = Settings.OpenInteractableInventoryType.Furnace;
        FurnaceOpen = true;
        CurrentFurnace = gameObject;

        LoadItemIntoSlot(FurnaceUI.GetComponent<InventoryController>().InventorySlots[0], InputItem);
        LoadItemIntoSlot(FurnaceUI.GetComponent<InventoryController>().InventorySlots[1], OutputItem);
        LoadItemIntoSlot(FurnaceUI.GetComponent<InventoryController>().InventorySlots[2], FuelItem);
    }

    public void LoadItemIntoSlot(GameObject slot, Item item)
    {
        foreach (Item JSONitem in JSONLoader.JSONItems.ItemList)
        {
            if (item != null && item.ItemName.Trim() != "" && JSONitem.ItemID == item.ItemID)
            {
                GameObject selectedSlot = slot;
                selectedSlot.GetComponent<InventorySlotController>().SlotItem = new Item(item.ItemID, item.ItemName, item.SpriteName, item.Amount, item.Stackable, item.IsSeed, item.ItemValue, item.ItemDescription);
                selectedSlot.GetComponent<InventorySlotController>().UpdateGUI(Resources.Load<Sprite>($"Graphics/Sprites/{item.SpriteName}"));
                selectedSlot.GetComponent<InventorySlotController>().SlotEmpty = false;
            }
        }
    }

    public static void CloseUI()
    {
        foreach (Transform slot in FurnaceUI.transform)
        {
            if (slot.tag == "InventorySlot")
            {
                slot.GetComponent<InventorySlotController>().SlotItem = null;
                slot.GetComponent<InventorySlotController>().SlotEmpty = true;
                slot.GetComponent<InventorySlotController>().EmptyGUI();
            }
        }
        if (FurnaceUI.activeInHierarchy)
            FurnaceUI.SetActive(false);
        Settings.InteractableInventoryType = Settings.OpenInteractableInventoryType.Empty;
        CurrentFurnace = null;
    }

    private void CheckFurnaceStatus()
    {
        if ((FuelItem != null && Settings.FuelNameAndTime.ContainsKey(FuelItem.ItemName)) || CurrentFuelTime > 0f)
        {
            if (CurrentFuelTime <= 0f)
            {
                if (InputItem != null && Settings.SmeltingInputAndOutput.ContainsKey(InputItem.ItemName) && (OutputItem == null || OutputItem.Amount < Settings.MaxItemQuantityPerSlot))
                {
                    CurrentFuelTime = Settings.FuelNameAndTime[FuelItem.ItemName];
                    CurrentFuelItemDuration = Settings.FuelNameAndTime[FuelItem.ItemName];

                    if (FuelItem.Amount - 1 > 0)
                        FuelItem.Amount--;
                    else
                    {
                        FuelItem = null;
                    }

                    FurnaceAnimator.SetBool("Burning", true);
                    if (FurnaceOpen)
                    {
                        UpdateSlots();
                    }
                }
                else
                {
                    if (FurnaceOpen)
                        GameObject.FindGameObjectWithTag("FurnaceFuelSlider").GetComponent<Slider>().value = 0f;
                }
            }
            if (CurrentFuelTime > 0f)
            {
                CurrentFuelTime -= Time.fixedUnscaledDeltaTime;
                if (FurnaceOpen)
                    GameObject.FindGameObjectWithTag("FurnaceFuelSlider").GetComponent<Slider>().value = CurrentFuelTime / CurrentFuelItemDuration;
            }
            else
            {
                CurrentFuelTime = 0f;
                if (FurnaceOpen)
                    GameObject.FindGameObjectWithTag("FurnaceFuelSlider").GetComponent<Slider>().value = 0f;
            }
        }

        string outputItemName = "";
        if (InputItem != null && Settings.SmeltingInputAndOutput.ContainsKey(InputItem.ItemName)) {
            outputItemName = Settings.SmeltingInputAndOutput[InputItem.ItemName];
        }

        if (InputItem != null && Settings.SmeltingInputAndOutput.ContainsKey(InputItem.ItemName) && (OutputItem == null || OutputItem.Amount == 0 || (OutputItem.ItemName == outputItemName && OutputItem.Amount < Settings.MaxItemQuantityPerSlot)) && CurrentFuelTime > 0f)
        {
            if (CurrentSmeltingTime < TimeToSmelt)
            {
                CurrentSmeltingTime += Time.fixedUnscaledDeltaTime;
                if (FurnaceOpen)
                    GameObject.FindGameObjectWithTag("FurnaceInputSlider").GetComponent<Slider>().value = CurrentSmeltingTime / TimeToSmelt;
            }
            else
            {
                AddItemToOutput();
                RemoveItemFromInput();
                if (FurnaceOpen)
                {
                    UpdateSlots();
                }
                CurrentSmeltingTime = 0f;
            }
        }
        if (CurrentFuelTime <= 0f)
        {
            CurrentFuelTime = 0f;
            FurnaceAnimator.SetBool("Burning", false);

            if (FurnaceOpen)
            {
                GameObject.FindGameObjectWithTag("FurnaceFuelSlider").GetComponent<Slider>().value = 0f;
            }
        }
        if (InputItem == null || !Settings.SmeltingInputAndOutput.ContainsKey(InputItem.ItemName) || (CurrentFuelTime <= 0f && (FuelItem == null || !Settings.FuelNameAndTime.ContainsKey(FuelItem.ItemName))) || (OutputItem != null && OutputItem.Amount == Settings.MaxItemQuantityPerSlot))
        {
            CurrentSmeltingTime = 0f;
            if (FurnaceOpen)
                GameObject.FindGameObjectWithTag("FurnaceInputSlider").GetComponent<Slider>().value = 0f;
        }
    }

    private void AddItemToOutput()
    {
        if (OutputItem == null || OutputItem.Amount == 0)
        {
            foreach (Item item in JSONLoader.JSONItems.ItemList)
            {
                if (item.ItemName == Settings.SmeltingInputAndOutput[InputItem.ItemName])
                {
                    OutputItem = new Item(item.ItemID, item.ItemName, item.SpriteName, 1, item.Stackable, item.IsSeed, item.ItemValue, item.ItemDescription);
                }
            }
        }
        else if (OutputItem.Stackable && OutputItem.Amount < Settings.MaxItemQuantityPerSlot && OutputItem.ItemName == Settings.SmeltingInputAndOutput[InputItem.ItemName])
        {
            if (OutputItem.Amount + 1 <= Settings.MaxItemQuantityPerSlot)
            {
                OutputItem.Amount += 1;
            }
        }
    }

    private void RemoveItemFromInput()
    {
        if (InputItem.Amount - 1 > 0)
            InputItem.Amount--;
        else
            InputItem = null;
    }

    public int AddItemToInput(string itemName, int quantity)
    {
        int leftAmount = quantity;
        if (InputItem == null || InputItem.Amount == 0)
        {
            foreach (Item item in JSONLoader.JSONItems.ItemList)
            {
                if (item.ItemName == itemName)
                {
                    InputItem = new Item(item.ItemID, item.ItemName, item.SpriteName, quantity, item.Stackable, item.IsSeed, item.ItemValue, item.ItemDescription);
                }
            }
            leftAmount = 0;
        }
        else if (InputItem.Stackable && InputItem.Amount + quantity <= Settings.MaxItemQuantityPerSlot && InputItem.ItemName == itemName)
        {
            InputItem.Amount += quantity;
            leftAmount = 0;
        }
        else if (InputItem.Stackable && InputItem.Amount + quantity > Settings.MaxItemQuantityPerSlot && InputItem.ItemName == itemName)
        {
            leftAmount = quantity - (Settings.MaxItemQuantityPerSlot - InputItem.Amount);
            InputItem.Amount = Settings.MaxItemQuantityPerSlot;
        }
        UpdateSlots();
        return leftAmount;
    }

    public int AddItemToFuel(string itemName, int quantity)
    {
        int leftAmount = quantity;

        if (FuelItem == null || FuelItem.Amount == 0)
        {
            foreach (Item item in JSONLoader.JSONItems.ItemList)
            {
                if (item.ItemName == itemName)
                {
                    FuelItem = new Item(item.ItemID, item.ItemName, item.SpriteName, quantity, item.Stackable, item.IsSeed, item.ItemValue, item.ItemDescription);
                }
            }
            leftAmount = 0;
        }
        else if (FuelItem.Stackable && FuelItem.Amount + quantity <= Settings.MaxItemQuantityPerSlot && FuelItem.ItemName == itemName)
        {
            FuelItem.Amount += quantity;
            leftAmount = 0;
        }
        else if (FuelItem.Stackable && FuelItem.Amount + quantity > Settings.MaxItemQuantityPerSlot && FuelItem.ItemName == itemName)
        {
            leftAmount = quantity - (Settings.MaxItemQuantityPerSlot - FuelItem.Amount);
            FuelItem.Amount = Settings.MaxItemQuantityPerSlot;
        }
        UpdateSlots();
        return leftAmount;
    }

    public void RemoveItemFromInput(int leftAmount)
    {
        if (leftAmount > 0)
            InputItem.Amount = leftAmount;
        else
            InputItem = null;
        UpdateSlots();
    }

    public void RemoveItemFromOutput(int leftAmount)
    {
        if (leftAmount > 0)
            OutputItem.Amount = leftAmount;
        else
            OutputItem = null;
        UpdateSlots();
    }

    public void RemoveItemFromFuel(int leftAmount)
    {
        if (leftAmount > 0)
            FuelItem.Amount = leftAmount;
        else
            FuelItem = null;
        UpdateSlots();
    }

    private void UpdateSlots()
    {
        if (InputItem != null && InputItem.Amount > 0)
        {
            FurnaceUI.GetComponent<InventoryController>().InventorySlots[0].GetComponent<InventorySlotController>().SlotItem = InputItem;
            FurnaceUI.GetComponent<InventoryController>().InventorySlots[0].GetComponent<InventorySlotController>().SlotEmpty = false;
            FurnaceUI.GetComponent<InventoryController>().InventorySlots[0].GetComponent<InventorySlotController>().UpdateGUI(Resources.Load<Sprite>($"Graphics/Sprites/{InputItem.SpriteName}"));
        }
        else
        {
            FurnaceUI.GetComponent<InventoryController>().InventorySlots[0].GetComponent<InventorySlotController>().SlotItem = InputItem;
            FurnaceUI.GetComponent<InventoryController>().InventorySlots[0].GetComponent<InventorySlotController>().EmptyGUI();
            FurnaceUI.GetComponent<InventoryController>().InventorySlots[0].GetComponent<InventorySlotController>().SlotEmpty = true;
        }

        if (OutputItem != null && OutputItem.Amount > 0)
        {
            FurnaceUI.GetComponent<InventoryController>().InventorySlots[1].GetComponent<InventorySlotController>().SlotItem = OutputItem;
            FurnaceUI.GetComponent<InventoryController>().InventorySlots[1].GetComponent<InventorySlotController>().SlotEmpty = false;
            FurnaceUI.GetComponent<InventoryController>().InventorySlots[1].GetComponent<InventorySlotController>().UpdateGUI(Resources.Load<Sprite>($"Graphics/Sprites/{OutputItem.SpriteName}"));
        }
        else
        {
            FurnaceUI.GetComponent<InventoryController>().InventorySlots[1].GetComponent<InventorySlotController>().SlotItem = OutputItem;
            FurnaceUI.GetComponent<InventoryController>().InventorySlots[1].GetComponent<InventorySlotController>().EmptyGUI();
            FurnaceUI.GetComponent<InventoryController>().InventorySlots[1].GetComponent<InventorySlotController>().SlotEmpty = true;

        }

        if (FuelItem != null && FuelItem.Amount > 0)
        {
            FurnaceUI.GetComponent<InventoryController>().InventorySlots[2].GetComponent<InventorySlotController>().SlotItem = FuelItem;
            FurnaceUI.GetComponent<InventoryController>().InventorySlots[2].GetComponent<InventorySlotController>().SlotEmpty = false;
            FurnaceUI.GetComponent<InventoryController>().InventorySlots[2].GetComponent<InventorySlotController>().UpdateGUI(Resources.Load<Sprite>($"Graphics/Sprites/{FuelItem.SpriteName}"));
        }
        else
        {
            FurnaceUI.GetComponent<InventoryController>().InventorySlots[2].GetComponent<InventorySlotController>().SlotItem = FuelItem;
            FurnaceUI.GetComponent<InventoryController>().InventorySlots[2].GetComponent<InventorySlotController>().EmptyGUI();
            FurnaceUI.GetComponent<InventoryController>().InventorySlots[2].GetComponent<InventorySlotController>().SlotEmpty = true;
        }
    }
}
