using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceTurretController : MonoBehaviour
{
    public GameObject FollowedEntity;
    public Animator TurretAnimator;
    public ParticleSystem TurretParticleSystem;
    public AudioSource TurretAudioSource;
    public AudioClip ShootingSound;

    public bool ShootingEnabled = false;

    float CurrentTime = 0f;

    private void FixedUpdate()
    {
        if (FollowedEntity != null)
        {
            if (ShootingEnabled)
            {
                CalculateRotationAndParticles();

                if (CurrentTime < Settings.TurretShootingInterval)
                {
                    CurrentTime += Time.fixedUnscaledDeltaTime;
                }
                else
                {
                    CurrentTime = 0f;
                    Shoot();
                }
            }
        }
        else
        {
            SearchForEnemy();
        }

        if (FollowedEntity != null && Vector2.Distance(transform.position, FollowedEntity.transform.position) > Settings.DistanceForTurretToDetectEntities)
        {
            FollowedEntity = null;
        }
    }

    private void SearchForEnemy()
    {
        Collider2D[] objectsInRadius = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), Settings.DistanceForTurretToDetectEntities);
        foreach (Collider2D collider in objectsInRadius)
        {
            if (collider.tag == "EnemyEntity")
            {
                FollowedEntity = collider.gameObject;
                break;
            }
        }
    }

    private void Shoot()
    {
        TurretParticleSystem.Play();
        SetAudioIntensity();
        TurretAudioSource.PlayOneShot(ShootingSound);
        FollowedEntity.GetComponent<AlienHealthController>().DamageAlien(Settings.DefenceTurretDamage);
    }

    private void SetAudioIntensity()
    {
        if (Vector2.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) > Settings.DistanceForThingsToBeAudible)
        {
            TurretAudioSource.volume = 0f;
        }
        else
        {
            TurretAudioSource.volume = (1 - (Vector2.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) / Settings.DistanceForThingsToBeAudible)) * 0.2f;
        }
    }

    private void CalculateRotationAndParticles()
    {
        Vector3 direction = transform.position - FollowedEntity.transform.position;
        direction.z = 0f;

        float degreesBetweenTurretAndEntity = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        var particleForceOverLifeTime = TurretParticleSystem.velocityOverLifetime;

        if (degreesBetweenTurretAndEntity < 22.5f && degreesBetweenTurretAndEntity >= -22.5f)
        {
            TurretParticleSystem.gameObject.transform.localPosition = Settings.DefenceTurretPositionLeft;
            TurretAnimator.Play("DefenceTurretLeft");
            TurretParticleSystem.GetComponent<ParticleSystemRenderer>().sortingOrder = 145;
        }
        else if (degreesBetweenTurretAndEntity < -22.5f && degreesBetweenTurretAndEntity >= -67.5f)
        {
            TurretParticleSystem.gameObject.transform.localPosition = Settings.DefenceTurretPositionLeftTop;
            TurretAnimator.Play("DefenceTurretLeftTop");
            TurretParticleSystem.GetComponent<ParticleSystemRenderer>().sortingOrder = 145;
        }
        else if (degreesBetweenTurretAndEntity < -67.5f && degreesBetweenTurretAndEntity >= -112.5f)
        {
            TurretParticleSystem.gameObject.transform.localPosition = Settings.DefenceTurretPositionTop;
            TurretAnimator.Play("DefenceTurretTop");
            TurretParticleSystem.GetComponent<ParticleSystemRenderer>().sortingOrder = 145;
        }
        else if (degreesBetweenTurretAndEntity < -112.5f && degreesBetweenTurretAndEntity >= -157.5f)
        {
            TurretParticleSystem.gameObject.transform.localPosition = Settings.DefenceTurretPositionRightTop;
            TurretAnimator.Play("DefenceTurretRightTop");
            TurretParticleSystem.GetComponent<ParticleSystemRenderer>().sortingOrder = 145;
        }
        else if (degreesBetweenTurretAndEntity < -157.5f || degreesBetweenTurretAndEntity > 157.5f)
        {
            TurretParticleSystem.gameObject.transform.localPosition = Settings.DefenceTurretPositionRight;
            TurretAnimator.Play("DefenceTurretRight");
            TurretParticleSystem.GetComponent<ParticleSystemRenderer>().sortingOrder = 145;
        }
        else if (degreesBetweenTurretAndEntity >= 112.5f && degreesBetweenTurretAndEntity < 157.5f)
        {
            TurretParticleSystem.gameObject.transform.localPosition = Settings.DefenceTurretPositionRightBottom;
            TurretAnimator.Play("DefenceTurretRightBottom");
            TurretParticleSystem.GetComponent<ParticleSystemRenderer>().sortingOrder = 200;
        }
        else if (degreesBetweenTurretAndEntity >= 67.5f && degreesBetweenTurretAndEntity < 112.5f)
        {
            TurretParticleSystem.gameObject.transform.localPosition = Settings.DefenceTurretPositionBottom;
            TurretAnimator.Play("DefenceTurretBottom");
            TurretParticleSystem.GetComponent<ParticleSystemRenderer>().sortingOrder = 200;
        }
        else
        {
            TurretParticleSystem.gameObject.transform.localPosition = Settings.DefenceTurretPositionLeftBottom;
            TurretAnimator.Play("DefenceTurretLeftBottom");
            TurretParticleSystem.GetComponent<ParticleSystemRenderer>().sortingOrder = 200;
        }



        particleForceOverLifeTime.xMultiplier =
            degreesBetweenTurretAndEntity < 45f && degreesBetweenTurretAndEntity > -45f ? -1f :
            degreesBetweenTurretAndEntity < -135f || degreesBetweenTurretAndEntity > 135f ? 1f :
            degreesBetweenTurretAndEntity < -45f && degreesBetweenTurretAndEntity > -135f ? ((90f + degreesBetweenTurretAndEntity) / 45f) * -1 :
            degreesBetweenTurretAndEntity > 45f && degreesBetweenTurretAndEntity < 135f ? ((-90f + degreesBetweenTurretAndEntity) / 45f) : 0f;
        particleForceOverLifeTime.yMultiplier =
            degreesBetweenTurretAndEntity < 45f && degreesBetweenTurretAndEntity > -45f ? (degreesBetweenTurretAndEntity < 0f ? (-1 + (degreesBetweenTurretAndEntity - 45) / -45) : 1 - (degreesBetweenTurretAndEntity + 45) / 45) :
            degreesBetweenTurretAndEntity < -135f || degreesBetweenTurretAndEntity > 135f ? (degreesBetweenTurretAndEntity < -135f ? (1 - (degreesBetweenTurretAndEntity + 135) / -45) : -1 + (degreesBetweenTurretAndEntity - 135) / 45) :
            degreesBetweenTurretAndEntity < -45f && degreesBetweenTurretAndEntity > -135f ? 1f :
            degreesBetweenTurretAndEntity > 45f && degreesBetweenTurretAndEntity < 135f ? -1f : 0f;
    }

}
