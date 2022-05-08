using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingController : MonoBehaviour
{
    public GameObject BuildingScheme;
    public SpriteRenderer BuildingSchemeRenderer;
    public Tilemap BuildingTilemap;
    string CurrentItemName = "";

    public void UseBuilder(string itemName, string spriteName)
    {
        if (CurrentItemName != itemName)
        {
            CurrentItemName = itemName;
            BuildingSchemeRenderer.sprite = Resources.Load<Sprite>($"Graphics/Sprites/{spriteName}");
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 normalizedMousePosition = new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), 0);

        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(normalizedMousePosition, Vector2.zero);
        BuildingScheme.transform.position = normalizedMousePosition;

        bool canBuild = true;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.GetComponent<Deconstructor>() != null)
            {
                canBuild = false;
            }
        }

        Vector3Int tilePos = BuildingTilemap.WorldToCell(mousePosition);
        Tile tile = BuildingTilemap.GetTile<Tile>(tilePos);
        if (tile != null) 
        {
            canBuild = false;
        }
        
        
        if (canBuild)
        {
            BuildingSchemeRenderer.color = Color.green;
            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(Resources.Load<GameObject>(Buildings.NameAndPath[itemName]), normalizedMousePosition, Quaternion.identity);
                InventoryController.RemoveItemQuantityFromSlot(InventoryController.ActiveHotbarSlot, 1);
                GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<AudioPlayer>().PlaySound("buildingSound");
            }
        }
        else
        {
            BuildingSchemeRenderer.color = Color.red;
        }
    }
}
