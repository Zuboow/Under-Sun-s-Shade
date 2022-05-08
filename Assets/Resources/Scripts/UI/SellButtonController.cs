using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellButtonController : MonoBehaviour
{
    public void SellItems()
    {
        int valueOfProducts = 0;

        foreach (Item item in ChestController.CurrentChest.GetComponent<ChestController>().Contents)
        {
            if (item != null)
                valueOfProducts += item.ItemValue * item.Amount;
        }

        if (valueOfProducts > 0)
        {
            for (int x = 0; x < ChestController.CurrentChest.GetComponent<ChestController>().Contents.Count; x++)
            {
                ChestController.CurrentChest.GetComponent<ChestController>().Contents[x] = null;
            }
            ChestController.CurrentChest.GetComponent<ChestController>().Interactable = false;
            ChestController.CurrentChest.GetComponent<TradingChestController>().UseDrone();
            ChestController.CurrentChest.GetComponent<TradingChestController>().CurrencyGained = valueOfProducts;

            ChestController.RemoveItemsFromSlots();
            InventoryController.CloseInventory();

            Debug.Log(valueOfProducts);
        }
    }
}
