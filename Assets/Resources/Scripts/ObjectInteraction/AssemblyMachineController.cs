using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssemblyMachineController : MonoBehaviour
{
    public Item Input1, Input2, Input3, Output;
    public static GameObject AssemblyMachineUI, CurrentAssemblyMachine;
    public bool AssemblyMachineOpen, RecipeIngredientsLoaded;
    public Animator AssemblyMachineAnimator;
    public Recipe CurrentRecipe = null;

    private float CurrentCraftingTime = 0f;
    public string SelectedItemName = "";

    private void OnEnable()
    {
        if (AssemblyMachineUI == null)
        {
            AssemblyMachineUI = GameObject.FindGameObjectWithTag("AssemblyMachineInventory");
            AssemblyMachineUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(Settings.InventoryButton) && /*Settings.InteractableInventoryType != Settings.OpenInteractableInventoryType.Empty &&*/ AssemblyMachineOpen)
        {
            CloseUI();
            AssemblyMachineOpen = false;
        }

        if (Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) > Settings.DistanceForObjectsToBeInteractable && transform.GetChild(0).gameObject.activeInHierarchy)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        CheckAssemblyMachineStatus();
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
        AssemblyMachineUI.SetActive(true);
        Settings.InteractableInventoryType = Settings.OpenInteractableInventoryType.AssemblyMachine;
        AssemblyMachineOpen = true;
        CurrentAssemblyMachine = gameObject;
        RecipeIngredientsLoaded = false;

        LoadItemIntoSlot(AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[0], Input1);
        LoadItemIntoSlot(AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[1], Input2);
        LoadItemIntoSlot(AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[2], Input3);
        LoadItemIntoSlot(AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[3], Output);
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
        foreach (Transform slot in AssemblyMachineUI.transform)
        {
            if (slot.tag == "InventorySlot")
            {
                slot.GetComponent<InventorySlotController>().SlotItem = null;
                slot.GetComponent<InventorySlotController>().SlotEmpty = true;
                slot.GetComponent<InventorySlotController>().EmptyGUI();
            }
        }
        if (AssemblyMachineUI.activeInHierarchy)
            AssemblyMachineUI.SetActive(false);
        Settings.InteractableInventoryType = Settings.OpenInteractableInventoryType.Empty;
        CurrentAssemblyMachine = null;
    }

    private void CheckAssemblyMachineStatus()
    {
        if (AssemblyMachineOpen)
        {
            if ((CurrentRecipe == null || CurrentRecipe.CraftedItemName != SelectedItemName) && SelectedItemName.Trim() != "")
            {
                foreach (Recipe recipe in JSONLoader.JSONRecipes.RecipeList)
                {
                    if (recipe.CraftedItemName == SelectedItemName)
                    {
                        CurrentRecipe = recipe;
                    }
                }
            }
            else if (!RecipeIngredientsLoaded && CurrentRecipe != null && SelectedItemName.Trim() != "")
            {
                AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[2].GetComponent<InventorySlotController>().DesiredItemName = CurrentRecipe.IngredientNames[0] == "none" ? "" : LoadSpriteName(CurrentRecipe.IngredientNames[0]);
                AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[1].GetComponent<InventorySlotController>().DesiredItemName = CurrentRecipe.IngredientNames[1] == "none" ? "" : LoadSpriteName(CurrentRecipe.IngredientNames[1]);
                AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[0].GetComponent<InventorySlotController>().DesiredItemName = CurrentRecipe.IngredientNames[2] == "none" ? "" : LoadSpriteName(CurrentRecipe.IngredientNames[2]);
                AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[3].GetComponent<InventorySlotController>().DesiredItemName = CurrentRecipe.CraftedItemName == "none" ? "" : LoadSpriteName(CurrentRecipe.CraftedItemName);

                AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[2].GetComponent<InventorySlotController>().UpdateDesiredItemSprite(Input3);
                AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[1].GetComponent<InventorySlotController>().UpdateDesiredItemSprite(Input2);
                AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[0].GetComponent<InventorySlotController>().UpdateDesiredItemSprite(Input1);
                AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[3].GetComponent<InventorySlotController>().UpdateDesiredItemSprite(Output);

                RecipeIngredientsLoaded = true;
            }
        }
        if (CurrentRecipe != null && (Output == null || Output.ItemName == "" || (Output.Amount < Settings.MaxItemQuantityPerSlot && Output.Stackable && Output.ItemName == CurrentRecipe.CraftedItemName)) &&
            CurrentRecipe.IngredientNames.Length > 0 && CurrentRecipe.IngredientQuantities.Length > 0 &&
            (CurrentRecipe.IngredientNames[0] == "none" || (Input3 != null && Input3.ItemName == CurrentRecipe.IngredientNames[0] && Input3.Amount >= CurrentRecipe.IngredientQuantities[0])) &&
            (CurrentRecipe.IngredientNames[1] == "none" || (Input2 != null && Input2.ItemName == CurrentRecipe.IngredientNames[1] && Input2.Amount >= CurrentRecipe.IngredientQuantities[1])) &&
            (CurrentRecipe.IngredientNames[2] == "none" || (Input1 != null && Input1.ItemName == CurrentRecipe.IngredientNames[2] && Input1.Amount >= CurrentRecipe.IngredientQuantities[2])))
        {
            if (CurrentCraftingTime < 5f)
            {
                CurrentCraftingTime += Time.unscaledDeltaTime;
                if (AssemblyMachineOpen)
                    GameObject.FindGameObjectWithTag("AssemblyMachineOutputSlider").GetComponent<Slider>().value = CurrentCraftingTime / 5f;
            }
            else
            {
                AddItemToOutput();
                RemoveItemsFromInput();
                if (AssemblyMachineOpen)
                {
                    UpdateSlots();
                }
                CurrentCraftingTime = 0f;
                if (AssemblyMachineOpen)
                    GameObject.FindGameObjectWithTag("AssemblyMachineOutputSlider").GetComponent<Slider>().value = 0f;
            }
        }
        else
        {
            CurrentCraftingTime = 0f;
            if (AssemblyMachineOpen)
                GameObject.FindGameObjectWithTag("AssemblyMachineOutputSlider").GetComponent<Slider>().value = 0f;
        }
    }

    private string LoadSpriteName(string itemName)
    {
        foreach (Item item in JSONLoader.JSONItems.ItemList)
        {
            if (item.ItemName == itemName)
            {
                return item.SpriteName;
            }
        }
        return "";
    }

    private void AddItemToOutput()
    {
        if (Output == null || Output.Amount == 0)
        {
            foreach (Item item in JSONLoader.JSONItems.ItemList)
            {
                if (item.ItemName == CurrentRecipe.CraftedItemName)
                {
                    Output = new Item(item.ItemID, item.ItemName, item.SpriteName, 1, item.Stackable, item.IsSeed, item.ItemValue, item.ItemDescription);
                }
            }
        }
        else if (Output.Stackable && Output.Amount < Settings.MaxItemQuantityPerSlot && Output.ItemName == CurrentRecipe.CraftedItemName)
        {
            if (Output.Amount + 1 <= Settings.MaxItemQuantityPerSlot)
            {
                Output.Amount += 1;
            }
        }
        RecipeIngredientsLoaded = false;
    }

    private void RemoveItemsFromInput()
    {
        if (Input1 != null)
        {
            if (Input1.Amount - CurrentRecipe.IngredientQuantities[2] > 0)
                Input1.Amount -= CurrentRecipe.IngredientQuantities[2];
            else
                Input1 = null;
        }

        if (Input2 != null)
        {
            if (Input2.Amount - CurrentRecipe.IngredientQuantities[1] > 0)
                Input2.Amount -= CurrentRecipe.IngredientQuantities[1];
            else
                Input2 = null;
        }

        if (Input3 != null)
        {
            if (Input3.Amount - CurrentRecipe.IngredientQuantities[0] > 0)
                Input3.Amount -= CurrentRecipe.IngredientQuantities[0];
            else
                Input3 = null;
        }
    }

    public int AddItemToInput(string itemName, int quantity)
    {
        int inputNumber = 0;
        if (Input3 == null || Input3.ItemName == "" || (Input3.ItemName == itemName && Input3.Amount < Settings.MaxItemQuantityPerSlot))
            inputNumber = 3;
        else if (Input2 == null || Input2.ItemName == "" || (Input2.ItemName == itemName && Input2.Amount < Settings.MaxItemQuantityPerSlot))
            inputNumber = 2;
        else if (Input1 == null || Input1.ItemName == "" || (Input1.ItemName == itemName && Input1.Amount < Settings.MaxItemQuantityPerSlot))
            inputNumber = 1;
        Item returnedItem = inputNumber == 1 ? Input1 : inputNumber == 2 ? Input2 : Input3;

        int leftAmount = quantity;
        if (returnedItem == null || returnedItem.ItemName == "" || returnedItem.Amount == 0)
        {
            foreach (Item item in JSONLoader.JSONItems.ItemList)
            {
                if (item.ItemName == itemName)
                {
                    switch (inputNumber)
                    {
                        case 1:
                            Input1 = new Item(item.ItemID, item.ItemName, item.SpriteName, quantity, item.Stackable, item.IsSeed, item.ItemValue, item.ItemDescription);
                            break;
                        case 2:
                            Input2 = new Item(item.ItemID, item.ItemName, item.SpriteName, quantity, item.Stackable, item.IsSeed, item.ItemValue, item.ItemDescription);
                            break;
                        case 3:
                            Input3 = new Item(item.ItemID, item.ItemName, item.SpriteName, quantity, item.Stackable, item.IsSeed, item.ItemValue, item.ItemDescription);
                            break;
                    }
                }
            }
            leftAmount = 0;
        }
        else if (returnedItem.Stackable && returnedItem.Amount + quantity <= Settings.MaxItemQuantityPerSlot && returnedItem.ItemName == itemName)
        {
            switch (inputNumber)
            {
                case 1:
                    Input1.Amount += quantity;
                    break;
                case 2:
                    Input2.Amount += quantity;
                    break;
                case 3:
                    Input3.Amount += quantity;
                    break;
            }
            leftAmount = 0;
        }
        else if (returnedItem.Stackable && returnedItem.Amount + quantity > Settings.MaxItemQuantityPerSlot && returnedItem.ItemName == itemName)
        {
            leftAmount = quantity - (Settings.MaxItemQuantityPerSlot - returnedItem.Amount);
            switch (inputNumber)
            {
                case 1:
                    Input1.Amount = Settings.MaxItemQuantityPerSlot;
                    break;
                case 2:
                    Input2.Amount = Settings.MaxItemQuantityPerSlot;
                    break;
                case 3:
                    Input3.Amount = Settings.MaxItemQuantityPerSlot;
                    break;
            }
        }
        UpdateSlots();
        return leftAmount;
    }

    public void RemoveItemFromInput(int leftAmount, string inputSlot)
    {
        switch (inputSlot)
        {
            case "input1":
                if (leftAmount > 0)
                    Input1.Amount = leftAmount;
                else
                    Input1 = null;
                break;
            case "input2":
                if (leftAmount > 0)
                    Input2.Amount = leftAmount;
                else
                    Input2 = null;
                break;
            case "input3":
                if (leftAmount > 0)
                    Input3.Amount = leftAmount;
                else
                    Input3 = null;
                break;
        }
        UpdateSlots();
    }

    public void RemoveItemFromOutput(int leftAmount)
    {
        if (leftAmount > 0)
            Output.Amount = leftAmount;
        else
            Output = null;
        UpdateSlots();
    }

    private void UpdateSlots()
    {
        if (Input1 != null && Input1.Amount > 0)
        {
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[0].GetComponent<InventorySlotController>().SlotItem = Input1;
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[0].GetComponent<InventorySlotController>().SlotEmpty = false;
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[0].GetComponent<InventorySlotController>().UpdateGUI(Resources.Load<Sprite>($"Graphics/Sprites/{Input1.SpriteName}"));
        }
        else
        {
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[0].GetComponent<InventorySlotController>().SlotItem = Input1;
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[0].GetComponent<InventorySlotController>().EmptyGUI();
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[0].GetComponent<InventorySlotController>().SlotEmpty = true;
        }

        if (Input2 != null && Input2.Amount > 0)
        {
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[1].GetComponent<InventorySlotController>().SlotItem = Input2;
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[1].GetComponent<InventorySlotController>().SlotEmpty = false;
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[1].GetComponent<InventorySlotController>().UpdateGUI(Resources.Load<Sprite>($"Graphics/Sprites/{Input2.SpriteName}"));
        }
        else
        {
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[1].GetComponent<InventorySlotController>().SlotItem = Input2;
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[1].GetComponent<InventorySlotController>().EmptyGUI();
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[1].GetComponent<InventorySlotController>().SlotEmpty = true;

        }

        if (Input3 != null && Input3.Amount > 0)
        {
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[2].GetComponent<InventorySlotController>().SlotItem = Input3;
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[2].GetComponent<InventorySlotController>().SlotEmpty = false;
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[2].GetComponent<InventorySlotController>().UpdateGUI(Resources.Load<Sprite>($"Graphics/Sprites/{Input3.SpriteName}"));
        }
        else
        {
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[2].GetComponent<InventorySlotController>().SlotItem = Input3;
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[2].GetComponent<InventorySlotController>().EmptyGUI();
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[2].GetComponent<InventorySlotController>().SlotEmpty = true;
        }

        if (Output != null && Output.Amount > 0)
        {
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[3].GetComponent<InventorySlotController>().SlotItem = Output;
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[3].GetComponent<InventorySlotController>().SlotEmpty = false;
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[3].GetComponent<InventorySlotController>().UpdateGUI(Resources.Load<Sprite>($"Graphics/Sprites/{Output.SpriteName}"));
        }
        else
        {
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[3].GetComponent<InventorySlotController>().SlotItem = Output;
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[3].GetComponent<InventorySlotController>().EmptyGUI();
            AssemblyMachineUI.GetComponent<InventoryController>().InventorySlots[3].GetComponent<InventorySlotController>().SlotEmpty = true;
        }
    }
}
