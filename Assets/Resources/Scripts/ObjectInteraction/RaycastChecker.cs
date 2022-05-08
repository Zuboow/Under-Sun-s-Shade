using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastChecker : MonoBehaviour
{
    void Update()
    {
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.tag == "InventorySlot")
            {
                hit.transform.GetComponent<InventorySlotController>().CheckRaycastHit();
            }
            if (hit.transform.tag == "Chest")
            {
                hit.transform.GetComponent<ChestController>().DecideOnMouseOver();
            }
            if (hit.transform.tag == "Furnace")
            {
                hit.transform.GetComponent<FurnaceController>().DecideOnMouseOver();
            }
        }
    }

}
