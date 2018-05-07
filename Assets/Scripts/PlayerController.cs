using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CustomPhysics {

    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    private SpriteRenderer spriteRenderer;
    private float hInput;
    private bool jumpInputDown;
    private bool jumpInputUp;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {

        hInput = Input.GetAxis("Horizontal");
        jumpInputDown = Input.GetKeyDown(KeyCode.RightControl);
        jumpInputUp = Input.GetKeyUp(KeyCode.RightControl);
    }

    protected override float ComputeVelocity() {
        Vector2 movement = Vector2.zero;

        movement.x = hInput;

        if (jumpInputDown && grounded) {
            velocity.y = jumpTakeOffSpeed;
        } else if (jumpInputUp && velocity.y > 0) {
            velocity.y = velocity.y * 0.5f;
        }

        bool flipSprite = (spriteRenderer.flipX ? (movement.x > 0.01f) : (movement.x < 0.01f));
        if (flipSprite) {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        return (movement * maxSpeed).x;
    }
}
