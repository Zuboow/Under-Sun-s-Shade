using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator PlayerAnimator;
    public Rigidbody2D PlayerRigidbody;

    private void Update()
    {
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        if (Settings.CanMove)
        {
            PlayerRigidbody.MovePosition(PlayerRigidbody.position + movement * Settings.PlayerSpeed * Time.fixedDeltaTime);

            PlayerAnimator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
            PlayerAnimator.SetFloat("Vertical", Input.GetAxis("Vertical"));
        }
    }
}
