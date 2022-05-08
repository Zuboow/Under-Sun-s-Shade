using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingItemButtonController : MonoBehaviour
{
    public int ItemID;
    public static GameObject CraftingDescription, CraftingItemName, CraftingButton;
    public static List<GameObject> CraftingIngredientsList = new List<GameObject>();

    private void Start()
    {
        CraftingDescription = GameObject.FindGameObjectWithTag("CraftingItemDescription");
        CraftingItemName = GameObject.FindGameObjectWithTag("CraftingItemName");
        if (CraftingIngredientsList.Count == 0)
        {
            CraftingIngredientsList.AddRange(GameObject.FindGameObjectsWithTag("CraftingItemIngredient"));
            foreach (GameObject ingredient in CraftingIngredientsList)
            {
                ingredient.SetActive(false);
            }
        }
        if (CraftingButton == null)
        {
            CraftingButton = GameObject.FindGameObjectWithTag("CraftButton");
            CraftingButton.SetActive(false);
        }
    }

    public void LoadDescriptionOnClick()
    {
        foreach (Item item in JSONLoader.JSONItems.ItemList)
        {
            if (item.ItemID == ItemID)
            {
                CraftingButton.SetActive(true);
                CraftingButton.GetComponent<CraftingButtonController>().ItemID = ItemID;
                CraftingDescription.GetComponent<TextMeshProUGUI>().text = item.ItemDescription;
                CraftingItemName.GetComponent<TextMeshProUGUI>().text = item.ItemName;

                foreach (Recipe recipe in JSONLoader.JSONRecipes.RecipeList)
                {
                    if (recipe.ItemID == item.ItemID)
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            if (recipe.IngredientNames[x] != "none")
                            {
                                CraftingIngredientsList[x].SetActive(true);

                                foreach (Item item_ in JSONLoader.JSONItems.ItemList)
                                {
                                    if (item_.ItemName == recipe.IngredientNames[x])
                                    {
                                        CraftingIngredientsList[x].transform.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Graphics/Sprites/{item_.SpriteName}");
                                        break;
                                    }
                                }
                                CraftingIngredientsList[x].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = recipe.IngredientNames[x];
                                CraftingIngredientsList[x].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = recipe.IngredientQuantities[x].ToString();
                            }
                            else
                            {
                                CraftingIngredientsList[x].SetActive(false);
                            }
                        }
                        CraftingButton.GetComponent<CraftingButtonController>().ItemNames = recipe.IngredientNames;
                        CraftingButton.GetComponent<CraftingButtonController>().ItemQuantities = recipe.IngredientQuantities;
                        CraftingButton.GetComponent<CraftingButtonController>().CraftedAmount = recipe.AmountCrafted;

                        break;
                    }
                }


            }
        }
    }
}
