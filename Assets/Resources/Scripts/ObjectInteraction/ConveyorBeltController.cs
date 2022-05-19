using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltController : MonoBehaviour
{
    public List<GameObject> Content = new List<GameObject>();
    public BuildingController.BuildingRotation Rotation = BuildingController.BuildingRotation.Bottom;
    public Animator ConveyorBeltAnimator;
    public float ItemSpeed = 1f;

    public bool AddItem = false;

    public void ChangeRotation()
    {
        switch (Rotation)
        {
            case BuildingController.BuildingRotation.Bottom:
                ConveyorBeltAnimator.Play("Bottom", -1, GameObject.FindGameObjectWithTag("ConveyorBelt").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);
                break;
            case BuildingController.BuildingRotation.Top:
                ConveyorBeltAnimator.Play("Top", -1, GameObject.FindGameObjectWithTag("ConveyorBelt").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);
                break;
            case BuildingController.BuildingRotation.Right:
                ConveyorBeltAnimator.Play("Right", -1, GameObject.FindGameObjectWithTag("ConveyorBelt").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);
                break;
            case BuildingController.BuildingRotation.Left:
                ConveyorBeltAnimator.Play("Left", -1, GameObject.FindGameObjectWithTag("ConveyorBelt").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (AddItem)
        {
            AddItemToBelt("Stone", new Vector2(0, .5f));
            AddItem = false;
        }

        MoveItemsByQueue();
    }

    private void SetQueue()
    {
        switch (Rotation)
        {
            case BuildingController.BuildingRotation.Bottom:
                ListSorter(new Vector2(0, -.5f));
                break;
            case BuildingController.BuildingRotation.Top:
                ListSorter(new Vector2(0, .5f));
                break;
            case BuildingController.BuildingRotation.Left:
                ListSorter(new Vector2(-.5f, 0));
                break;
            case BuildingController.BuildingRotation.Right:
                ListSorter(new Vector2(.5f, 0));
                break;
        }
    }

    private void ListSorter(Vector2 desiredPosition)
    {
        Content.Sort(delegate(GameObject firstPosition, GameObject secondPosition)
        {
            if (Vector2.Distance(firstPosition.transform.localPosition, desiredPosition) < Vector2.Distance(secondPosition.transform.localPosition, desiredPosition))
            {
                return -1;
            }
            else return 1;
        });
    }

    public bool AddItemToBelt(string itemName, Vector2 startingPosition)
    {
        if (Content.Count == 0 || Vector2.Distance(Content[Content.Count - 1].transform.localPosition, startingPosition) > Settings.MaxDistanceBetweenItemsOnConveyorBelt && Content.Count <= 4)
        {
            foreach (Item item in JSONLoader.JSONItems.ItemList)
            {
                if (item.ItemName == itemName)
                {
                    GameObject placedItem = Instantiate(Resources.Load<GameObject>("Prefabs/DroppedItem"), transform.position, Quaternion.identity);
                    placedItem.GetComponent<GrabbingController>().CurrentItem = new Item(item.ItemID, item.ItemName, item.SpriteName, 1, item.Stackable, item.IsSeed, item.ItemValue, item.ItemDescription);
                    placedItem.GetComponent<GrabbingController>().Grabbable = false;
                    placedItem.GetComponent<GrabbingController>().UpdateInfo();
                    placedItem.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Graphics/Sprites/{item.SpriteName}");
                    placedItem.transform.parent = transform;
                    placedItem.transform.localPosition = startingPosition;
                    
                    Content.Add(placedItem);
                    SetQueue();

                    return true;
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    private void MoveItem(GameObject item)
    {
        switch (Rotation)
        {
            case BuildingController.BuildingRotation.Bottom:
                item.transform.localPosition = Vector2.MoveTowards(item.transform.localPosition, new Vector2(0, -.5f), ItemSpeed * Time.fixedDeltaTime);
                if (item.transform.localPosition.y == -.5f)
                {
                    CheckNextBelt(item, new Vector3(0, -1, 0), new Vector2(0, .5f));
                }
                break;
            case BuildingController.BuildingRotation.Top:
                item.transform.localPosition = Vector2.MoveTowards(item.transform.localPosition, new Vector2(0, .5f), ItemSpeed * Time.fixedDeltaTime);
                if (item.transform.localPosition.y == .5f)
                {
                    CheckNextBelt(item, new Vector3(0, 1, 0), new Vector2(0, -.5f));
                }
                break;
            case BuildingController.BuildingRotation.Right:
                item.transform.localPosition = Vector2.MoveTowards(item.transform.localPosition, new Vector2(.5f, 0), ItemSpeed * Time.fixedDeltaTime);
                if (item.transform.localPosition.x == .5f)
                {
                    CheckNextBelt(item, new Vector3(1, 0, 0), new Vector2(-.5f, 0));
                }
                break;
            case BuildingController.BuildingRotation.Left:
                item.transform.localPosition = Vector2.MoveTowards(item.transform.localPosition, new Vector2(-.5f, 0), ItemSpeed * Time.fixedDeltaTime);
                if (item.transform.localPosition.x == -.5f)
                {
                    CheckNextBelt(item, new Vector3(-1, 0, 0), new Vector2(.5f, 0));
                }
                break;
        }
    }

    private void MoveItemsByQueue()
    {
        for (int x = 0; x < Content.Count; x++)
        {
            if (x == 0)
            {
                MoveItem(Content[x]);
            }
            else if (Vector2.Distance(Content[x].transform.localPosition, Content[x - 1].transform.localPosition) >= Settings.MaxDistanceBetweenItemsOnConveyorBelt)
            {
                MoveItem(Content[x]);
            }
        }
    }

    private void CheckNextBelt(GameObject item, Vector3 offsetVector, Vector2 startingPosition)
    {
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(transform.position + offsetVector, Vector2.zero);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.tag == "ConveyorBelt" && hit.transform.GetComponent<ConveyorBeltController>().Content.Count <= 4)
            {
                if (hit.transform.GetComponent<ConveyorBeltController>().AddItemToBelt(item.transform.GetComponent<GrabbingController>().CurrentItem.ItemName, startingPosition))
                {
                    Destroy(Content[0]);
                    Content.RemoveAt(0);
                }
            }
        }
    }
}
