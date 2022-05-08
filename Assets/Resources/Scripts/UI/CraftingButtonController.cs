using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingButtonController : MonoBehaviour
{
    public int ItemID, CraftedAmount;
    public int[] ItemQuantities;
    public string[] ItemNames;

    public void CraftItem()
    {
        bool canBeCrafted = true;

        for (int x = 0; x < 3; x++)
        {
            if (ItemNames[x] != "none")
            {
                if (!GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<ItemManagementController>().CheckIfItemQuantityIsPresent(ItemNames[x], ItemQuantities[x]))
                {
                    canBeCrafted = false;
                    break;
                }
            }
        }

        if (canBeCrafted)
        {
            GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<AudioPlayer>().PlaySound("craftingSound");

            for (int x = 0; x < 3; x++)
            {
                if (ItemNames[x] != "none")
                {
                    GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<ItemManagementController>().RemoveItemFromInventory(ItemNames[x], ItemQuantities[x]);
                }
            }

            foreach (Item item in JSONLoader.JSONItems.ItemList)
            {
                if (item.ItemID == ItemID)
                {
                    GameObject droppedItem = Instantiate(Resources.Load<GameObject>("Prefabs/DroppedItem"), GameObject.FindGameObjectWithTag("Player").transform.position, Quaternion.identity);
                    droppedItem.GetComponent<GrabbingController>().CurrentItem = new Item(item.ItemID, item.ItemName, item.SpriteName, CraftedAmount, item.Stackable, item.IsSeed, item.ItemValue, item.ItemDescription);
                    droppedItem.GetComponent<GrabbingController>().UpdateInfo();
                    droppedItem.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Graphics/Sprites/{item.SpriteName}");
                }
            }
        }
        else
        {
            GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<AudioPlayer>().PlaySound("error");

            Debug.LogError("Not enough resources");
        }
    }
}
