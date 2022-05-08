using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarSlotUser : MonoBehaviour
{
    public GameObject BuildingScheme;
    private void Update()
    {
        if (InventoryController.ActiveHotbarSlot.GetComponent<InventorySlotController>() != null && InventoryController.ActiveHotbarSlot.GetComponent<InventorySlotController>().SlotItem != null)
        {
            switch (InventoryController.ActiveHotbarSlot.GetComponent<InventorySlotController>().SlotItem.ItemName)
            {
                case "Pickaxe":
                    if (Input.GetMouseButton(0) && !InventoryController.InventoryOpen && Settings.CanMove)
                    {
                        GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>().Play("PickaxeHit", -1);
                        Settings.CanMove = false;
                    }
                    if (BuildingScheme.activeInHierarchy)
                        BuildingScheme.SetActive(false);
                    break;
                default:
                    if (BuildingScheme.activeInHierarchy)
                        BuildingScheme.SetActive(false);
                    break;

            }
            if (Buildings.NameAndPath.ContainsKey(InventoryController.ActiveHotbarSlot.GetComponent<InventorySlotController>().SlotItem.ItemName))
            {
                if (!InventoryController.InventoryOpen && Settings.CanMove)
                {
                    BuildingScheme.SetActive(true);
                    GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<BuildingController>().UseBuilder(InventoryController.ActiveHotbarSlot.GetComponent<InventorySlotController>().SlotItem.ItemName, InventoryController.ActiveHotbarSlot.GetComponent<InventorySlotController>().SlotItem.SpriteName);
                }
            }
        }
        else
        {
            if (BuildingScheme.activeInHierarchy)
                BuildingScheme.SetActive(false);
        }

    }
}
