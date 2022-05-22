using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltController : MonoBehaviour
{
    public List<GameObject> MainLine = new List<GameObject>(), LeftWaitingLine = new List<GameObject>(), RightWaitingLine = new List<GameObject>();
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
            AddItemToBelt("Stone", ReturnStartingPosition(), true);
            AddItem = false;
        }

        MoveItemsByQueue();
        DecideQueueOnWaitingLines(true);
        DecideQueueOnWaitingLines(false);
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
        MainLine.Sort(delegate (GameObject firstPosition, GameObject secondPosition)
        {
            if (Vector2.Distance(firstPosition.transform.localPosition, desiredPosition) < Vector2.Distance(secondPosition.transform.localPosition, desiredPosition))
            {
                return -1;
            }
            else return 1;
        });
        LeftWaitingLine.Sort(delegate (GameObject firstPosition, GameObject secondPosition)
        {
            if (Vector2.Distance(firstPosition.transform.localPosition, new Vector2(0, 0)) < Vector2.Distance(secondPosition.transform.localPosition, new Vector2(0, 0)))
            {
                return -1;
            }
            else return 1;
        });
        RightWaitingLine.Sort(delegate (GameObject firstPosition, GameObject secondPosition)
        {
            if (Vector2.Distance(firstPosition.transform.localPosition, new Vector2(0, 0)) < Vector2.Distance(secondPosition.transform.localPosition, new Vector2(0, 0)))
            {
                return -1;
            }
            else return 1;
        });
    }

    public bool AddItemToBelt(string itemName, Vector2 startingPosition, bool itemOnMainLine)
    {
        if (itemOnMainLine && (MainLine.Count == 0 || Vector2.Distance(MainLine[MainLine.Count - 1].transform.localPosition, startingPosition) > Settings.MaxDistanceBetweenItemsOnConveyorBelt) || !itemOnMainLine && CheckIfItemCanGoToWaitingLine(startingPosition))
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

                    if (!itemOnMainLine)
                    {
                        switch (Rotation)
                        {
                            case BuildingController.BuildingRotation.Bottom:
                                if (startingPosition.x < 0) RightWaitingLine.Add(placedItem); else LeftWaitingLine.Add(placedItem);
                                break;
                            case BuildingController.BuildingRotation.Top:
                                if (startingPosition.x < 0) LeftWaitingLine.Add(placedItem); else RightWaitingLine.Add(placedItem);
                                break;
                            case BuildingController.BuildingRotation.Right:
                                if (startingPosition.y < 0) RightWaitingLine.Add(placedItem); else LeftWaitingLine.Add(placedItem);
                                break;
                            default:
                                if (startingPosition.y < 0) LeftWaitingLine.Add(placedItem); else RightWaitingLine.Add(placedItem);
                                break;
                        }
                    }
                    else MainLine.Add(placedItem);

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

    private void DecideQueueOnWaitingLines(bool leftLine)
    {
        for (int x = 0; x < (leftLine ? LeftWaitingLine.Count : RightWaitingLine.Count); x++)
        {
            GameObject item = (leftLine ? LeftWaitingLine[x] : RightWaitingLine[x]);

            if (x == 0 ||
                (leftLine && Vector2.Distance(LeftWaitingLine[x].transform.localPosition, LeftWaitingLine[x - 1].transform.localPosition) > Settings.MaxDistanceBetweenItemsOnConveyorBelt * 0.6f) ||
                (!leftLine && Vector2.Distance(RightWaitingLine[x].transform.localPosition, RightWaitingLine[x - 1].transform.localPosition) > Settings.MaxDistanceBetweenItemsOnConveyorBelt * 0.6f))
            {
                switch (Rotation)
                {
                    case BuildingController.BuildingRotation.Bottom:
                        if (item.transform.localPosition.x != 0)
                        {
                            if ((item.transform.localPosition.x <= 0 && item.transform.localPosition.x < -0.2f) || (item.transform.localPosition.x >= 0 && item.transform.localPosition.x > 0.2f))
                                item.transform.localPosition = Vector2.MoveTowards(item.transform.localPosition, new Vector2(0, item.transform.localPosition.y), ItemSpeed * Time.fixedUnscaledDeltaTime);
                            else
                            {
                                AddItemToMainLine(leftLine);
                            }
                        }
                        break;
                    case BuildingController.BuildingRotation.Top:
                        if (item.transform.localPosition.x != 0)
                        {
                            if ((item.transform.localPosition.x <= 0 && item.transform.localPosition.x < -0.2f) || (item.transform.localPosition.x >= 0 && item.transform.localPosition.x > 0.2f))
                                item.transform.localPosition = Vector2.MoveTowards(item.transform.localPosition, new Vector2(0, item.transform.localPosition.y), ItemSpeed * Time.fixedUnscaledDeltaTime);
                            else
                            {
                                AddItemToMainLine(leftLine);
                            }
                        }
                        break;
                    case BuildingController.BuildingRotation.Right:
                        if (item.transform.localPosition.y != 0)
                        {
                            if ((item.transform.localPosition.y <= 0 && item.transform.localPosition.y < -0.2f) || (item.transform.localPosition.y >= 0 && item.transform.localPosition.y > 0.2f))
                                item.transform.localPosition = Vector2.MoveTowards(item.transform.localPosition, new Vector2(item.transform.localPosition.x, 0), ItemSpeed * Time.fixedUnscaledDeltaTime);
                            else
                            {
                                AddItemToMainLine(leftLine);
                            }
                        }
                        break;
                    default:
                        if (item.transform.localPosition.y != 0)
                        {
                            if ((item.transform.localPosition.y <= 0 && item.transform.localPosition.y < -0.2f) || (item.transform.localPosition.y >= 0 && item.transform.localPosition.y > 0.2f))
                                item.transform.localPosition = Vector2.MoveTowards(item.transform.localPosition, new Vector2(item.transform.localPosition.x, 0), ItemSpeed * Time.fixedUnscaledDeltaTime);
                            else
                            {
                                AddItemToMainLine(leftLine);
                            }
                        }
                        break;
                }
            }
        }
    }

    private void MoveItem(GameObject item)
    {
        switch (Rotation)
        {
            case BuildingController.BuildingRotation.Bottom:
                if (item.transform.localPosition.x != 0)
                    item.transform.localPosition = Vector2.MoveTowards(item.transform.localPosition, new Vector2(0, item.transform.localPosition.y), ItemSpeed * Time.fixedUnscaledDeltaTime);
                else
                    item.transform.localPosition = Vector2.MoveTowards(item.transform.localPosition, new Vector2(0, -.5f), ItemSpeed * Time.fixedUnscaledDeltaTime);
                if (item.transform.localPosition.y == -.5f)
                {
                    CheckNextBelt(item, new Vector3(0, -1, 0), new Vector2(0, .5f));
                }
                break;
            case BuildingController.BuildingRotation.Top:
                if (item.transform.localPosition.x != 0)
                    item.transform.localPosition = Vector2.MoveTowards(item.transform.localPosition, new Vector2(0, item.transform.localPosition.y), ItemSpeed * Time.fixedUnscaledDeltaTime);
                else
                    item.transform.localPosition = Vector2.MoveTowards(item.transform.localPosition, new Vector2(0, .5f), ItemSpeed * Time.fixedUnscaledDeltaTime);
                if (item.transform.localPosition.y == .5f)
                {
                    CheckNextBelt(item, new Vector3(0, 1, 0), new Vector2(0, -.5f));
                }
                break;
            case BuildingController.BuildingRotation.Right:
                if (item.transform.localPosition.y != 0)
                    item.transform.localPosition = Vector2.MoveTowards(item.transform.localPosition, new Vector2(item.transform.localPosition.x, 0), ItemSpeed * Time.fixedUnscaledDeltaTime);
                else
                    item.transform.localPosition = Vector2.MoveTowards(item.transform.localPosition, new Vector2(.5f, 0), ItemSpeed * Time.fixedUnscaledDeltaTime);
                if (item.transform.localPosition.x == .5f)
                {
                    CheckNextBelt(item, new Vector3(1, 0, 0), new Vector2(-.5f, 0));
                }
                break;
            case BuildingController.BuildingRotation.Left:
                if (item.transform.localPosition.y != 0)
                    item.transform.localPosition = Vector2.MoveTowards(item.transform.localPosition, new Vector2(item.transform.localPosition.x, 0), ItemSpeed * Time.fixedUnscaledDeltaTime);
                else
                    item.transform.localPosition = Vector2.MoveTowards(item.transform.localPosition, new Vector2(-.5f, 0), ItemSpeed * Time.fixedUnscaledDeltaTime);
                if (item.transform.localPosition.x == -.5f)
                {
                    CheckNextBelt(item, new Vector3(-1, 0, 0), new Vector2(.5f, 0));
                }
                break;
        }
    }

    private void MoveItemsByQueue()
    {
        for (int x = 0; x < MainLine.Count; x++)
        {
            if (x == 0)
            {
                MoveItem(MainLine[x]);
            }
            else if (Vector2.Distance(MainLine[x].transform.localPosition, MainLine[x - 1].transform.localPosition) > Settings.MaxDistanceBetweenItemsOnConveyorBelt)
            {
                MoveItem(MainLine[x]);
            }
        }
    }

    private void CheckNextBelt(GameObject item, Vector3 offsetVector, Vector2 startingPosition)
    {
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(transform.position + offsetVector, Vector2.zero);

        foreach (RaycastHit2D hit in hits)
        {
            ConveyorBeltController controller = hit.transform.GetComponent<ConveyorBeltController>();
            if (hit.transform.tag == "ConveyorBelt" && (controller.MainLine.Count == 0 || Vector2.Distance(controller.MainLine[controller.MainLine.Count - 1].transform.position, MainLine[0].transform.position) > Settings.MaxDistanceBetweenItemsOnConveyorBelt))
            {
                if (CheckIfBeltHasRevertedRotation(hit.transform.GetComponent<ConveyorBeltController>().Rotation))
                {
                    if (hit.transform.GetComponent<ConveyorBeltController>().AddItemToBelt(item.transform.GetComponent<GrabbingController>().CurrentItem.ItemName, startingPosition, CheckIfItemGoesToMainLine(hit.transform.GetComponent<ConveyorBeltController>().Rotation)))
                    {
                        Destroy(MainLine[0]);
                        MainLine.RemoveAt(0);
                    }
                }
            }
        }
    }

    private void AddItemToMainLine(bool leftLine)
    {
        bool canAdd = true;

        foreach (GameObject item in MainLine)
        {
            if (Vector2.Distance(item.transform.localPosition, new Vector2(0, 0)) < Settings.MaxDistanceBetweenItemsOnConveyorBelt * 0.6f)
                canAdd = false;
        }

        if (canAdd)
        {
            MainLine.Add(leftLine ? LeftWaitingLine[0] : RightWaitingLine[0]);
            if (leftLine) LeftWaitingLine.RemoveAt(0); else RightWaitingLine.RemoveAt(0);
            SetQueue();
        }
    }

    private bool CheckIfBeltHasRevertedRotation(BuildingController.BuildingRotation hitRotation)
    {
        if (hitRotation == BuildingController.BuildingRotation.Bottom && Rotation == BuildingController.BuildingRotation.Top) return false;
        if (hitRotation == BuildingController.BuildingRotation.Top && Rotation == BuildingController.BuildingRotation.Bottom) return false;
        if (hitRotation == BuildingController.BuildingRotation.Left && Rotation == BuildingController.BuildingRotation.Right) return false;
        if (hitRotation == BuildingController.BuildingRotation.Right && Rotation == BuildingController.BuildingRotation.Left) return false;
        return true;
    }

    private bool CheckIfItemGoesToMainLine(BuildingController.BuildingRotation hitRotation)
    {
        if (hitRotation == BuildingController.BuildingRotation.Bottom && (Rotation == BuildingController.BuildingRotation.Left || Rotation == BuildingController.BuildingRotation.Right)) return false;
        if (hitRotation == BuildingController.BuildingRotation.Top && (Rotation == BuildingController.BuildingRotation.Left || Rotation == BuildingController.BuildingRotation.Right)) return false;
        if (hitRotation == BuildingController.BuildingRotation.Left && (Rotation == BuildingController.BuildingRotation.Top || Rotation == BuildingController.BuildingRotation.Bottom)) return false;
        if (hitRotation == BuildingController.BuildingRotation.Right && (Rotation == BuildingController.BuildingRotation.Top || Rotation == BuildingController.BuildingRotation.Bottom)) return false;
        return true;
    }

    private bool CheckIfItemCanGoToWaitingLine(Vector2 startingPosition)
    {
        switch (Rotation)
        {
            case BuildingController.BuildingRotation.Bottom:
                if (startingPosition.x < 0 && RightWaitingLine.Count < 2) return true; else if (startingPosition.x > 0 && LeftWaitingLine.Count < 2) return true; else return false;
            case BuildingController.BuildingRotation.Top:
                if (startingPosition.x < 0 && LeftWaitingLine.Count < 2) return true; else if (startingPosition.x > 0 && RightWaitingLine.Count < 2) return true; else return false;
            case BuildingController.BuildingRotation.Right:
                if (startingPosition.y < 0 && RightWaitingLine.Count < 2) return true; else if (startingPosition.y > 0 && LeftWaitingLine.Count < 2) return true; else return false;
            default:
                if (startingPosition.y < 0 && LeftWaitingLine.Count < 2) return true; else if (startingPosition.y > 0 && RightWaitingLine.Count < 2) return true; else return false;
        }
    }

    private Vector2 ReturnStartingPosition()
    {
        switch (Rotation)
        {
            case BuildingController.BuildingRotation.Bottom:
                return new Vector2(0, .5f);
            case BuildingController.BuildingRotation.Left:
                return new Vector2(.5f, 0);
            case BuildingController.BuildingRotation.Right:
                return new Vector2(-.5f, 0);
            default:
                return new Vector2(0, -.5f);
        }
    }
}
