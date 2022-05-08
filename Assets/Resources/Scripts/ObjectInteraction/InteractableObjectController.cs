using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectController : MonoBehaviour
{
    public Sprite OriginalSprite, ChangedSprite;
    public List<string> ItemsToDrop = new List<string>();
    public bool ReadyToDispense = true, ContinousUse;
    public DispenseType TypeOfDispense;
    public int TicksNeededToReset, Ticks = 0;

    public enum DispenseType
    {
        UseByTool,
        UseByMouse
    }

    public void OnMouseOver()
    {
        if (Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) < Settings.DistanceForInteractableObjectToDetectHovering) {
            if (Settings.CheckCollisionOnAnimationHit && ReadyToDispense && TypeOfDispense == DispenseType.UseByTool)
            {
                Settings.CheckCollisionOnAnimationHit = false;
                DropItemsAndChangeSprite(ContinousUse);
                if (!ContinousUse)
                    ReadyToDispense = false;
            }
            if (TypeOfDispense == DispenseType.UseByMouse)
            {
                transform.GetChild(0).gameObject.SetActive(true);

                if (Input.GetKeyDown(Settings.UseButton) && ReadyToDispense)
                {
                    DropItemsAndChangeSprite(ContinousUse);
                    if (!ContinousUse)
                        ReadyToDispense = false;
                }
                if (!ReadyToDispense)
                    transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        else if (TypeOfDispense == DispenseType.UseByMouse)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void OnMouseExit()
    {
        if (TypeOfDispense == DispenseType.UseByMouse)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void DropItemsAndChangeSprite(bool continousUse)
    {
        foreach (string itemName in ItemsToDrop)
        {
            foreach (Item item in JSONLoader.JSONItems.ItemList)
            {
                if (item.ItemName == itemName)
                {
                    Vector2 randomizedPosition = Random.insideUnitCircle * Settings.DistanceRangeOfItemDropping;
                    GameObject droppedItem = Instantiate(Resources.Load<GameObject>("Prefabs/DroppedItem"), new Vector2(transform.position.x, transform.position.y) + randomizedPosition, Quaternion.identity);
                    droppedItem.GetComponent<GrabbingController>().CurrentItem = new Item(item.ItemID, item.ItemName, item.SpriteName, 1, item.Stackable, item.IsSeed, item.ItemValue, item.ItemDescription);
                    droppedItem.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Graphics/Sprites/{item.SpriteName}");
                }
            }
        }
        if (!continousUse)
        {
            GetComponent<SpriteRenderer>().sprite = ChangedSprite;
            gameObject.layer = 2; //IgnoreRaycast
        }
    }

    public void AddTick()
    {
        Ticks += 1;
        if (Ticks >= TicksNeededToReset)
            ResetObject();
    }

    public void ResetObject()
    {
        Ticks = 0;
        GetComponent<SpriteRenderer>().sprite = OriginalSprite;
        ReadyToDispense = true;
        gameObject.layer = 0; //Default
    }
}
