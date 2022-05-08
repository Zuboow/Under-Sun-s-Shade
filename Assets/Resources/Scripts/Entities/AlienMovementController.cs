using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienMovementController : MonoBehaviour
{
    public GameObject TargetedObject;
    public Rigidbody2D AlienRigidbody;
    public Animator AlienAnimator;

    public float AlienSpeed = 2f, NextWaypointDistance = 3f;
    Path AlienPath;
    int CurrentWaypoint;
    Seeker AlienSeeker;

    bool CanRotate = true;

    private void OnEnable()
    {
        AlienSeeker = GetComponent<Seeker>();
        InvokeRepeating("UpdatePath", 0f, 0.3f);
    }

    private void FixedUpdate()
    {
        if (TargetedObject == null)
        {
            TargetedObject = null;
            AlienAnimator.Play("SmallAlien_Idle", -1);
            SearchForEnemy();
        }
        else if (Vector2.Distance(TargetedObject.transform.position, transform.position) > 1f)
        {
            UsePathfinding();
        }
        else
        {
            AlienAnimator.Play("SmallAlien_Idle", -1);
        }
    }

    void UpdatePath()
    {
        if (AlienSeeker.IsDone() && TargetedObject != null)
        {
            AlienSeeker.StartPath(AlienRigidbody.position, TargetedObject.transform.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            AlienPath = p;
            CurrentWaypoint = 0;
        }
    }

    private void SearchForEnemy()
    {
        Collider2D[] objectsInRadius = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), Settings.DistanceForAlienToDetectPlayerOrBuildings);
        foreach (Collider2D collider in objectsInRadius)
        {
            if (collider.tag == "Player")
            {
                Debug.Log("Player in radius");
                TargetedObject = collider.gameObject;
                break;
            }
        }
    }

    public void FinishAnimation()
    {
        CanRotate = true;
    }

    private void AnimateAlien(SpriteAnimationRotator.AnimationRotation rotationResult)
    {
        switch (rotationResult)
        {
            case SpriteAnimationRotator.AnimationRotation.Down:
                AlienAnimator.Play("SmallAlien_Down", -1);
                break;
            case SpriteAnimationRotator.AnimationRotation.Up:
                AlienAnimator.Play("SmallAlien_Up", -1);
                break;
            case SpriteAnimationRotator.AnimationRotation.Left:
                AlienAnimator.Play("SmallAlien_Left", -1);
                break;
            case SpriteAnimationRotator.AnimationRotation.Right:
                AlienAnimator.Play("SmallAlien_Right", -1);
                break;
        }
        CanRotate = false;
    }

    public void UsePathfinding()
    {
        if (AlienPath == null)
            return;
        if (CurrentWaypoint >= AlienPath.vectorPath.Count)
        {
            return;
        }

        Vector2 direction = ((Vector2)AlienPath.vectorPath[CurrentWaypoint] - AlienRigidbody.position).normalized;
        Vector2 force = direction * AlienSpeed * Time.deltaTime;
        if (CanRotate)
            AnimateAlien(SpriteAnimationRotator.GetDirectionByAngle(-direction));

        AlienRigidbody.AddForce(force);

        float distance = Vector2.Distance(AlienRigidbody.position, AlienPath.vectorPath[CurrentWaypoint]);

        if (distance < NextWaypointDistance)
        {
            CurrentWaypoint++;
        }
    }
}
