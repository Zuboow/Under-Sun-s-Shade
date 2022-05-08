using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Player;
    private Vector3 CameraPosition;
    public float ShakeDuration;
    public bool Shake = false;

    void OnEnable()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, -10f);
        if (Shake) ShakeCamera();
    }

    public void StartShaking()
    {
        ShakeDuration = 0.25f;
        Shake = true;
        CameraPosition = transform.localPosition;
        CameraPosition.z = -10f;
    }
    public void ShakeCamera()
    {
        if (ShakeDuration > 0f)
        {
            transform.localPosition = CameraPosition + Random.insideUnitSphere * 0.15f;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -10f);

            ShakeDuration -= Time.deltaTime;
        }
        else
        {
            ShakeDuration = 0f;
            transform.localPosition = CameraPosition;
            Shake = false;
        }
    }
}
