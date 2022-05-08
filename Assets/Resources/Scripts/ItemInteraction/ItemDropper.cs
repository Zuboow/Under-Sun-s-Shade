using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            DropItem("Furnace", 1);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            DropItem("Chest", 1);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            DropItem("Wheat Seeds", 2);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            DropItem("DefenceTurret", 2);
        }
    }

    public void DropItem(string itemName, int quantity)
    {
        foreach (Item item in JSONLoader.JSONItems.ItemList)
        {
            if (item.ItemName == itemName)
            {
                GameObject droppedItem = Instantiate(Resources.Load<GameObject>("Prefabs/DroppedItem"), GameObject.FindGameObjectWithTag("Player").transform.position, Quaternion.identity);
                droppedItem.GetComponent<GrabbingController>().CurrentItem = new Item(item.ItemID, item.ItemName, item.SpriteName, quantity, item.Stackable, item.IsSeed, item.ItemValue, item.ItemDescription);
                droppedItem.GetComponent<GrabbingController>().DroppedFromInventory = true;
                droppedItem.GetComponent<GrabbingController>().UpdateInfo();
                droppedItem.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Graphics/Sprites/{item.SpriteName}");
            }
        }
    }
}
