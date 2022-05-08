using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienHealthController : MonoBehaviour
{
    public int AlienCurrentHealth, AlienMaxHealth = 200;
    public GameObject DeadAlienPrefab;
    

    private void OnEnable()
    {
        AlienCurrentHealth = AlienMaxHealth;
    }

    public void DamageAlien(int damageReceived)
    {
        if (AlienCurrentHealth - damageReceived > 0)
        {
            AlienCurrentHealth -= damageReceived;
        }
        else
        {
            AlienCurrentHealth = 0;
            Instantiate(DeadAlienPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }

    
}
