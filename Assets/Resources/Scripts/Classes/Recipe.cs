using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Recipe
{
    public string CraftedItemName;
    public int ItemID, AmountCrafted;
    public string[] IngredientNames;
    public int[] IngredientQuantities;

    public Recipe(string _craftedItemName, int _itemID, string[] _ingredientNames, int[] _ingredientQuantities, int _amountCrafted)
    {
        CraftedItemName = _craftedItemName;
        ItemID = _itemID;
        IngredientNames = _ingredientNames;
        IngredientQuantities = _ingredientQuantities;
        AmountCrafted = _amountCrafted;
    }
}

[System.Serializable]
public class Recipes
{
    public Recipe[] RecipeList;
}
