using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GrabbingController : MonoBehaviour
{
    public Item CurrentItem = null;
    public static List<Item> AllSlots = new List<Item>();
    private bool InfoUpdated = false, CoroutineStarted = false;
    public bool DroppedFromInventory = false, Grabbable = true;

    private void Update()
    {
        if (DroppedFromInventory)
        {
            if (!CoroutineStarted)
            {
                CoroutineStarted = true;
                StartCoroutine(WaitAfterDroppingFromInventory());
            }
        }
        else
        {
            if (CurrentItem != null && !InfoUpdated)
            {
                UpdateInfo();
            }
            if (Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) < Settings.DistanceForItemsToSlideTowardsPlayer && CheckIfItemCanBeTaken(CurrentItem.ItemID) && Grabbable)
            {
                if (Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) > Settings.DistanceForItemsToAddToPlayerInventory)
                {
                    transform.position = Vector2.MoveTowards(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position, Settings.ItemSlideSpeed * Time.deltaTime);
                }
                else
                {
                    int itemsLeft = GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<ItemManagementController>().AddItemToInventory(CurrentItem.ItemName, CurrentItem.Amount);

                    if (itemsLeft == 0)
                    {
                        Destroy(gameObject);
                        transform.GetChild(0).gameObject.SetActive(false);
                        GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<AudioPlayer>().PlaySound("grabbing");
                    }
                    else if (itemsLeft != CurrentItem.Amount)
                    {
                        CurrentItem.Amount = itemsLeft;
                        EditItemList(GameObject.FindGameObjectWithTag("InventoryContainer"), GameObject.FindGameObjectWithTag("HotbarContainer"));
                        GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<AudioPlayer>().PlaySound("grabbing");
                    }
                }
            }
        }
    }

    public static void EditItemList(GameObject inventorySlotsContainer, GameObject hotbarSlotsContainer)
    {
        AllSlots = new List<Item>();

        foreach (GameObject slot in inventorySlotsContainer.GetComponent<InventoryController>().InventorySlots)
        {
            AllSlots.Add(slot.GetComponent<InventorySlotController>().SlotItem);
        }
        foreach (GameObject slot in hotbarSlotsContainer.GetComponent<InventoryController>().InventorySlots)
        {
            AllSlots.Add(slot.GetComponent<InventorySlotController>().SlotItem);
        }
    }

    public static bool CheckIfItemCanBeTaken(int itemID)
    {
        foreach (Item item in AllSlots)
        {
            if (item == null)
            {
                return true;
            }
            else if (item.ItemID == itemID)
            {
                if (item.Amount < Settings.MaxItemQuantityPerSlot && item.Stackable)
                {
                    return true;
                }
            }
        }

        return false;
    }

    IEnumerator WaitAfterDroppingFromInventory()
    {
        yield return new WaitForSeconds(Settings.WaitingTimeAfterDroppingFromInventory);
        DroppedFromInventory = false;
        StopAllCoroutines();
    }

    public void UpdateInfo()
    {
        transform.GetChild(0).GetComponent<TextMeshPro>().text = $"{CurrentItem.ItemName}({CurrentItem.Amount})";
    }

    private void OnMouseExit()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void OnMouseOver()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
