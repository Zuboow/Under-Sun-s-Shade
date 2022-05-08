using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingViewController : MonoBehaviour
{
    public List<string> ItemsAvailableForCrafting = new List<string>();
    public GameObject CraftingItemPrefab, ListContents, CraftingContainer;
    public static bool CraftingOpen = true;

    private void OnEnable()
    {
        CraftingContainer.transform.localPosition = Settings.ClosedCraftingPosition;
        CraftingOpen = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(Settings.CraftingViewButton))
        {
            CraftingOpen = !CraftingOpen;

            if (CraftingOpen)
            {
                TurnCraftingVisibility(true);
                if (!InventoryController.InventoryOpen)
                    InventoryController.OpenInventory();
            }
            else
            {
                if (InventoryController.InventoryOpen)
                {
                    FurnaceController.CloseUI();
                    InventoryController.CloseInventory();
                }
                TurnCraftingVisibility(false);
            }
        }
        if (Input.GetKeyDown(Settings.InventoryButton) && CraftingOpen)
        {
            CraftingOpen = false;
            TurnCraftingVisibility(false);
        }
    }

    public void TurnCraftingVisibility(bool open)
    {
        CraftingContainer.transform.localPosition = open ? Settings.OpenCraftingPosition : Settings.ClosedCraftingPosition;
        if (open)
        {
            LoadItems();
        }
    }

    public void LoadItems()
    {
        foreach (Transform child in ListContents.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (string itemName in ItemsAvailableForCrafting)
        {
            foreach (Item item in JSONLoader.JSONItems.ItemList)
            {
                if (item.ItemName == itemName)
                {
                    GameObject itemPrefab = Instantiate(CraftingItemPrefab, ListContents.transform.position, Quaternion.identity);
                    itemPrefab.transform.parent = ListContents.transform;
                    itemPrefab.transform.localScale = new Vector3(1, 1, 1);
                    itemPrefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.ItemName;
                    itemPrefab.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Graphics/Sprites/{item.SpriteName}");
                    itemPrefab.GetComponent<CraftingItemButtonController>().ItemID = item.ItemID;
                }
            }
        }
    }
}
