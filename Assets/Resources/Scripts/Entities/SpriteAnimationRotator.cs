using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimationRotator : MonoBehaviour
{
    public enum AnimationRotation
    {
        Up,
        Down,
        Left,
        Right
    }

    public static AnimationRotation GetDirectionByAngle(Vector3 direction)
    {
        direction.z = 0f;

        float degreesBetweenTargetAndEntity = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (degreesBetweenTargetAndEntity < 45f && degreesBetweenTargetAndEntity >= -45f)
        {
            return AnimationRotation.Left;
        }
        if (degreesBetweenTargetAndEntity < 45f && degreesBetweenTargetAndEntity >= -135f)
        {
            return AnimationRotation.Up;
        }
        if (degreesBetweenTargetAndEntity < -135f || degreesBetweenTargetAndEntity >= 135f)
        {
            return AnimationRotation.Right;
        }
        if (degreesBetweenTargetAndEntity < 135f && degreesBetweenTargetAndEntity >= 45f)
        {
            return AnimationRotation.Down;
        }
        else
        {
            return AnimationRotation.Down;
        }
    }
}
