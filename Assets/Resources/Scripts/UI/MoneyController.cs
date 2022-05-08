using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyController : MonoBehaviour
{
    public static GameObject MoneyText;
    public static int Money = 0;

    private void OnEnable()
    {
        MoneyText = GameObject.FindGameObjectWithTag("MoneyUIText");
        MoneyText.GetComponent<TextMeshProUGUI>().text = Money.ToString();
    }

    public static void AddMoney(int amount)
    {
        Money += amount;
        MoneyText.GetComponent<TextMeshProUGUI>().text = Money.ToString();
    }
}
