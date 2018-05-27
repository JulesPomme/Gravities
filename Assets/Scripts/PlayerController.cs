using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CustomPhysics {

    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    private SpriteRenderer spriteRenderer;
    private bool jump;
    private bool slowJump;
    private float hInput;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        jump = false;
        slowJump = false;
        hInput = 0f;
    }

    private void Update() {

        bool jumpInputDown = Input.GetKeyDown(KeyCode.RightControl);
        bool jumpInputUp = Input.GetKeyUp(KeyCode.RightControl);

        //Register a jump entry 
        if (jumpInputDown && grounded && !jump) {
            jump = true;
        }
        //Register a jump release (slow down jump)
        slowJump = jumpInputUp && velocity.y > 0;

        //Register a walk entry
        hInput = Input.GetAxis("Horizontal");
    }

    protected override Vector2 ComputeVelocity() {

        Vector2 targetVelocity = Vector2.zero;

        //Jump
        if (jump) {
            targetVelocity.y = jumpTakeOffSpeed;
            jump = false;
        } else if (slowJump) {
            targetVelocity.y = velocity.y * 0.5f;
        } else {
            targetVelocity.y = velocity.y;
        }

        //Walk
        targetVelocity.x = hInput * maxSpeed;

        bool flipSprite = (spriteRenderer.flipX ? (hInput > 0.01f) : (hInput < 0.01f));
        if (flipSprite) {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        return targetVelocity;

    }

    public override void SetGravity(Vector2 newGravity) {

        base.SetGravity(newGravity);
        Debug.Log("coucou");
    }

    //protected override void ComputeVelocity() {

    //    //Jump
    //    bool jumpInputDown = Input.GetKeyDown(KeyCode.RightControl);
    //    bool jumpInputUp = Input.GetKeyUp(KeyCode.RightControl);
    //    if (jumpInputDown && grounded) {
    //        velocity.y = jumpTakeOffSpeed;
    //    } else if (jumpInputUp && velocity.y > 0) {
    //        velocity.y = velocity.y * 0.5f;
    //    }

    //    //Walk
    //    float hInput = Input.GetAxis("Horizontal");
    //    Vector2 movement = Vector2.zero;
    //    movement.x = hInput;

    //    bool flipSprite = (spriteRenderer.flipX ? (movement.x > 0.01f) : (movement.x < 0.01f));
    //    if (flipSprite) {
    //        spriteRenderer.flipX = !spriteRenderer.flipX;
    //    }

    //    targetVelocity = movement * maxSpeed;
    //}
}
