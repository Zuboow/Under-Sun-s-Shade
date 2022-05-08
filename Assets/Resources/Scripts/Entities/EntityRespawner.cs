using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityRespawner : MonoBehaviour
{
    public int TicksNeededToReset, Ticks = 0;
    public GameObject Entity;

    private void OnEnable()
    {
        Ticks = TicksNeededToReset;
    }

    private void Update()
    {
        if (transform.childCount == 0 && Ticks == TicksNeededToReset)
        {
            DropItem(Entity);
            Ticks = 0;
        }
    }

    public void DropItem(GameObject entity)
    {
        GameObject spawnedEntity = Instantiate(entity, transform.position, Quaternion.identity);
        spawnedEntity.transform.parent = transform;
    }

    public void AddTick()
    {
        Ticks += 1;
    }
}
