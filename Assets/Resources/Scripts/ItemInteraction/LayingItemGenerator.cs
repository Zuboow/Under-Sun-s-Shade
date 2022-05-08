using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayingItemGenerator : MonoBehaviour
{
    public int TicksNeededToReset, Ticks = 0, Quantity;
    public string ItemName;

    private void OnEnable()
    {
        Ticks = TicksNeededToReset;
    }

    private void Update()
    {
        if (transform.childCount == 0 && Ticks == TicksNeededToReset) 
        {
            DropItem(ItemName, Quantity);
            Ticks = 0;
        }
    }

    public void DropItem(string itemName, int quantity)
    {
        foreach (Item item in JSONLoader.JSONItems.ItemList)
        {
            if (item.ItemName == ItemName)
            {
                GameObject droppedItem = Instantiate(Resources.Load<GameObject>("Prefabs/DroppedItem"), transform.position, Quaternion.identity);
                droppedItem.GetComponent<GrabbingController>().CurrentItem = new Item(item.ItemID, item.ItemName, item.SpriteName, quantity, item.Stackable, item.IsSeed, item.ItemValue, item.ItemDescription);
                droppedItem.GetComponent<GrabbingController>().DroppedFromInventory = true;
                droppedItem.GetComponent<GrabbingController>().UpdateInfo();
                droppedItem.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Graphics/Sprites/{item.SpriteName}");
                droppedItem.transform.parent = transform;
            }
        }
    }

    public void AddTick()
    {
        Ticks += 1;
    }
}
