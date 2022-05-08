using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradingChestController : MonoBehaviour
{
    public Animator TradingChestAnimator;
    public GameObject Drone;
    public bool DroneOn = false;
    public int CurrencyGained = 0;

    public void ReplaceChest()
    {
        TradingChestAnimator.Play("ChestReplace", -1);
    }

    public void EmptyChest()
    {
        TradingChestAnimator.Play("ChestEmpty", -1);
        GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<AudioPlayer>().PlaySound("moneyAdd");
        MoneyController.AddMoney(CurrencyGained);
    }

    public void ChestReady()
    {
        TradingChestAnimator.Play("ChestIdle", -1);
        GetComponent<ChestController>().Interactable = true;
    }

    public void UseDrone()
    {
        Drone.transform.position = new Vector3(transform.position.x, transform.position.y + 20f);
        DroneOn = true;
        Drone.SetActive(true);
        Drone.GetComponent<TradingDroneMovement>().State = TradingDroneMovement.DroneState.Deploying;
    }
}
