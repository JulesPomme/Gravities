using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CustomPhysics {

    public enum Direction {
        X, mX, Y, mY, XY, mXY, XmY, mXmY
    }

    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    private SpriteRenderer spriteRenderer;
    private bool jump;
    private bool slowJump;
    private float walkInput;
    private Vector2 previousGravity;
    private Direction gravityDirection;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        jump = false;
        slowJump = false;
        walkInput = 0f;
        previousGravity = gravity;
        gravityDirection = Direction.mY;
    }

    private void Update() {

        bool jumpInputDown = Input.GetKeyDown(KeyCode.LeftControl);
        bool jumpInputUp = Input.GetKeyUp(KeyCode.LeftControl);

        //Register a jump entry 
        if (jumpInputDown && grounded && !jump) {
            jump = true;
        }
        //Register a jump release (slow down jump)
        slowJump = jumpInputUp && velocity.y > 0;

        //Update gravity main direction only when gravity is changing AND player is grounded (in order to let CustomPhysics update its current normal).
        if (previousGravity != gravity && grounded) {
            gravityDirection = GetGravityMainDirection();
            previousGravity = gravity;
        }
        //Register a walk entry. Input depends on the gravity direction.
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");
        bool chooseHInput = gravityDirection == Direction.mY || gravityDirection == Direction.XmY && Mathf.Abs(hInput) >= Math.Abs(vInput) || gravityDirection == Direction.mXmY && Mathf.Abs(hInput) >= Math.Abs(vInput);
        bool chooseMinusHInput = gravityDirection == Direction.Y || gravityDirection == Direction.XY && Mathf.Abs(hInput) >= Math.Abs(vInput) || gravityDirection == Direction.mXY && Mathf.Abs(hInput) >= Math.Abs(vInput);
        bool chooseVInput = gravityDirection == Direction.X || gravityDirection == Direction.XY && Mathf.Abs(vInput) > Math.Abs(hInput) || gravityDirection == Direction.XmY && Mathf.Abs(vInput) > Math.Abs(hInput);
        bool chooseMinusVInput = gravityDirection == Direction.mX || gravityDirection == Direction.mXY && Mathf.Abs(vInput) > Math.Abs(hInput) || gravityDirection == Direction.mXmY && Mathf.Abs(vInput) > Math.Abs(hInput);
        if (chooseHInput) {
            walkInput = hInput;
        } else if (chooseMinusHInput) {
            walkInput = -hInput;
        } else if (chooseVInput) {
            walkInput = vInput;
        } else if (chooseMinusVInput) {
            walkInput = -vInput;
        }
    }

    private Direction GetGravityMainDirection() {

        Direction dir = Direction.mY;
        float gravityY = Mathf.Round(gravity.y * 100f) / 100f;
        float gravityX = Mathf.Round(gravity.x * 100f) / 100f;
        if (Mathf.Abs(gravityY) > Mathf.Abs(gravityX)) {
            dir = gravityY < 0 ? Direction.mY : Direction.Y;
        } else if (Mathf.Abs(gravityX) > Mathf.Abs(gravityY)) {
            dir = gravityX < 0 ? Direction.mX : Direction.X;
        } else {//Mathf.Abs(gravityY) == Mathf.Abs(gravityX)
            if (gravityY > 0) {
                if (gravityX > 0) {
                    dir = Direction.XY;
                } else {
                    dir = Direction.mXY;
                }
            } else {
                if (gravityX > 0) {
                    dir = Direction.XmY;
                } else {
                    dir = Direction.mXmY;
                }
            }
        }
        return dir;
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
        targetVelocity.x = walkInput * maxSpeed;

        bool flipSprite = (spriteRenderer.flipX ? (walkInput > 0.01f) : (walkInput < 0.01f));
        if (flipSprite) {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        return targetVelocity;

    }

    public override void SetGravity(Vector2 newGravity) {

        base.SetGravity(newGravity);
    }
}
