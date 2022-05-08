using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioClip 
        GrabbingSound, 
        PickaxeHit, 
        MoneyAdd,
        ChestOpening,
        CraftingSound,
        Error,
        BuildingSound;
    public AudioSource AudioSource;

    public void PlaySound(string soundType)
    {
        switch (soundType)
        {
            case "grabbing":
                AudioSource.PlayOneShot(GrabbingSound);
                break;
            case "pickaxe":
                AudioSource.PlayOneShot(PickaxeHit);
                break;
            case "moneyAdd":
                AudioSource.PlayOneShot(MoneyAdd);
                break;
            case "chestOpening":
                AudioSource.PlayOneShot(ChestOpening);
                break;
            case "craftingSound":
                AudioSource.PlayOneShot(CraftingSound);
                break;
            case "error":
                AudioSource.PlayOneShot(Error);
                break;
            case "buildingSound":
                AudioSource.PlayOneShot(BuildingSound);
                break;
        }
    }
}
