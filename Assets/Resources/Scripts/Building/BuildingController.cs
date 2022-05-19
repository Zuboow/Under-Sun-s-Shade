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
    public static BuildingRotation CurrentBuildingRotation = BuildingRotation.Bottom, PreviousBuildingRotation = BuildingRotation.Bottom;

    public enum BuildingRotation
    {
        Left,
        Right,
        Top, 
        Bottom
    }

    private void Update()
    {
        if (BuildingScheme.activeInHierarchy && Input.GetKeyDown(Settings.BuildingRotationButton))
        {
            switch (CurrentBuildingRotation)
            {
                case BuildingRotation.Bottom:
                    CurrentBuildingRotation = BuildingRotation.Left;
                    break;
                case BuildingRotation.Left:
                    CurrentBuildingRotation = BuildingRotation.Top;
                    break;
                case BuildingRotation.Top:
                    CurrentBuildingRotation = BuildingRotation.Right;
                    break;
                case BuildingRotation.Right:
                    CurrentBuildingRotation = BuildingRotation.Bottom;
                    break;
            }
        }
    }

    public void UseBuilder(string itemName, string spriteName)
    {
        if (CurrentItemName != itemName || PreviousBuildingRotation != CurrentBuildingRotation)
        {
            PreviousBuildingRotation = CurrentBuildingRotation;
            CurrentItemName = itemName;
            switch (itemName)
            {
                case "Basic Conveyor Belt":
                    BuildingSchemeRenderer.sprite = Resources.Load<Sprite>($"Graphics/Sprites/Objects/BasicConveyorBelt/{Sprites.ConveyorBelts[CurrentBuildingRotation]}");
                    break;
                case "Assembly Machine":
                    BuildingSchemeRenderer.sprite = Resources.Load<Sprite>($"Graphics/Sprites/Objects/AssemblyMachine/AssemblyMachine");
                    break;
                default:
                    BuildingSchemeRenderer.sprite = Resources.Load<Sprite>($"Graphics/Sprites/{spriteName}");
                    break;
            }
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
                GameObject newObject = Instantiate(Resources.Load<GameObject>(Buildings.NameAndPath[itemName]), normalizedMousePosition, Quaternion.identity);
                InventoryController.RemoveItemQuantityFromSlot(InventoryController.ActiveHotbarSlot, 1);
                GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<AudioPlayer>().PlaySound("buildingSound");

                CheckIfRotationIsPossible(newObject, newObject.tag);
            }
        }
        else
        {
            BuildingSchemeRenderer.color = Color.red;
        }
    }

    public void CheckIfRotationIsPossible(GameObject newObject, string tag)
    {
        if (tag == "ConveyorBelt")
        {
            newObject.GetComponent<ConveyorBeltController>().Rotation = CurrentBuildingRotation;
            newObject.GetComponent<ConveyorBeltController>().ChangeRotation();
        }
    }
}
