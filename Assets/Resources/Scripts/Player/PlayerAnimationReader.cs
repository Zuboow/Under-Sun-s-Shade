using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationReader : MonoBehaviour
{
    public void ReadPickaxeHit()
    {
        Settings.CheckCollisionOnAnimationHit = true;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().StartShaking();
        GameObject.FindGameObjectWithTag("ScriptExecutor").GetComponent<AudioPlayer>().PlaySound("pickaxe");
    }

    public void Reset()
    {
        Settings.CanMove = true;
        Settings.CheckCollisionOnAnimationHit = false;
    }
}
