using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int ItemID, Amount, ItemValue;
    public string ItemName, SpriteName, ItemDescription;
    public bool Stackable, IsSeed;

    public Item(int _itemID, string _itemName, string _spriteName, int _amount, bool _stackable, bool _isSeed, int _itemValue, string _itemDescription)
    {
        ItemID = _itemID;
        ItemName = _itemName;
        SpriteName = _spriteName;
        Amount = _amount;
        Stackable = _stackable;
        IsSeed = _isSeed;
        ItemValue = _itemValue;
        ItemDescription = _itemDescription;
    }
}

[System.Serializable]
public class Items
{
    public Item[] ItemList;
}
