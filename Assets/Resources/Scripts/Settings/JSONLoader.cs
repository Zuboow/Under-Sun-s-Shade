using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONLoader : MonoBehaviour
{
    public static Items JSONItems;
    public static Recipes JSONRecipes;

    private void OnEnable()
    {
        JSONItems = JsonUtility.FromJson<Items>((Resources.Load("JSON/items") as TextAsset).text);
        JSONRecipes = JsonUtility.FromJson<Recipes>((Resources.Load("JSON/recipes") as TextAsset).text);
    }
}
