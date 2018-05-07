using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CustomPhysics {

    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    private SpriteRenderer spriteRenderer;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void ComputeVelocity() {

        //Jump
        bool jumpInputDown = Input.GetKeyDown(KeyCode.RightControl);
        bool jumpInputUp = Input.GetKeyUp(KeyCode.RightControl);
        if (jumpInputDown && grounded) {
            velocity.y = jumpTakeOffSpeed;
        } else if (jumpInputUp && velocity.y > 0) {
            velocity.y = velocity.y * 0.5f;
        }

        //Walk
        float hInput = Input.GetAxis("Horizontal");
        Vector2 movement = Vector2.zero;
        movement.x = hInput;

        bool flipSprite = (spriteRenderer.flipX ? (movement.x > 0.01f) : (movement.x < 0.01f));
        if (flipSprite) {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        targetVelocity = movement * maxSpeed;
    }
}
