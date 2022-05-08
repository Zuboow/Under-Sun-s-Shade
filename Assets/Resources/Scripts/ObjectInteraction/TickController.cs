using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickController : MonoBehaviour
{
    public static float TickRate = 1f, CurrentTickTime = 0f;

    void Update()
    {
        CalculateTicks();
    }

    public static void CalculateTicks()
    {
        if (TickRate < CurrentTickTime)
        {
            CurrentTickTime = 0;
            AddTick();
        }
        else
        {
            CurrentTickTime += Time.deltaTime;
        }
    }

    public static void AddTick()
    {
        List<GameObject> GameObjectsAffectedByTick = new List<GameObject>();
        GameObjectsAffectedByTick.AddRange(GameObject.FindGameObjectsWithTag("AffectedByTick"));

        foreach (GameObject gObject in GameObjectsAffectedByTick)
        {
            if (gObject.GetComponent<InteractableObjectController>() != null && !gObject.GetComponent<InteractableObjectController>().ReadyToDispense)
                gObject.GetComponent<InteractableObjectController>().AddTick();
            if (gObject.GetComponent<FarmingFieldController>() != null && gObject.GetComponent<FarmingFieldController>().FieldStage != FarmingFieldController.FarmStage.Empty &&
                gObject.GetComponent<FarmingFieldController>().FieldStage != FarmingFieldController.FarmStage.ReadyToHarvest)
                gObject.GetComponent<FarmingFieldController>().AddTick();
            if (gObject.GetComponent<LayingItemGenerator>() != null && gObject.transform.childCount == 0)
                gObject.GetComponent<LayingItemGenerator>().AddTick();
            if (gObject.GetComponent<EntityRespawner>() != null && gObject.transform.childCount == 0)
                gObject.GetComponent<EntityRespawner>().AddTick();
        }
    }
}
