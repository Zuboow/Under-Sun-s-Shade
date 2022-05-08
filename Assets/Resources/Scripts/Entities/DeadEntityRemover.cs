using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEntityRemover : MonoBehaviour
{
    public float CurrentTime = 0f;
    public AudioClip AlienDeathSound;
    public AudioSource AlienAudioSource;

    private void OnEnable()
    {
        SetAudioIntensity();
        AlienAudioSource.PlayOneShot(AlienDeathSound);
    }

    void Update()
    {
        if (CurrentTime < Settings.TimeForDeadEntityToBeRemoved)
        {
            CurrentTime += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetAudioIntensity()
    {
        if (Vector2.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) > Settings.DistanceForThingsToBeAudible)
        {
            AlienAudioSource.volume = 0f;
        }
        else
        {
            AlienAudioSource.volume = (1 - (Vector2.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) / Settings.DistanceForThingsToBeAudible)) * 0.2f;
        }
    }
}
