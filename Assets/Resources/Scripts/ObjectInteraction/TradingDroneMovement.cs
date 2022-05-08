using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradingDroneMovement : MonoBehaviour
{
    public GameObject Chest, GrabbedChest;
    public DroneState State = DroneState.Idle;
    public Animator DroneAnimator;

    public enum DroneState
    {
        Idle,
        Deploying,
        Empty,
        ChestGrabbed
    }

    private void Update()
    {
        switch (State)
        {
            case DroneState.Deploying:
                transform.position = Chest.transform.position + new Vector3(0, 20f);
                GrabbedChest.SetActive(false);
                State = DroneState.Empty;
                break;
            case DroneState.Empty:
                MoveDrone();
                break;
            case DroneState.ChestGrabbed:
                GoAway();
                break;

        }
    }

    public void MoveDrone()
    {
        if (Vector2.Distance(transform.position, Chest.transform.position) > 0.6f)
        {
            transform.position = transform.position + (new Vector3(0, -1f) * Time.deltaTime * 4f);
        }
        else
        {
            DroneAnimator.Play("DroneGrab", -1);
        }
    }

    public void SetReadyState()
    {
        State = DroneState.ChestGrabbed;
        GrabbedChest.SetActive(true);
        Chest.GetComponent<TradingChestController>().EmptyChest();
        DroneAnimator.Play("DroneGrabbing", -1);
    }

    public void GoAway()
    {
        if (Vector2.Distance(transform.position, Chest.transform.position) < 20f)
        {
            transform.position = transform.position + (new Vector3(0, 2f) * Time.deltaTime * 2f);
        }
        else
        {
            transform.parent.GetComponent<TradingChestController>().DroneOn = false;
            gameObject.SetActive(false);
        }
    }
}
