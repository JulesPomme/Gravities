using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CustomPhysics
{
    //TODO jump bug (quand on saute immédiatement après avoir atterit, on a parfois un double saut qui s'enclenche)

    public enum Direction
    {
        X, mX, Y, mY, XY, mXY, XmY, mXmY
    }

    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

	private Animator animator;

    private SpriteRenderer spriteRenderer;
    private bool jump;
    private bool slowJump;
    private float walkInput;
    //The registered gravity the last time the player was grounded.
    private Vector2 previousGravityAtGrounded;
    private Direction gravityDirection;
    private bool updateJumpVelocity;
    private float gravityDiff;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        jump = false;
        slowJump = false;
        walkInput = 0f;
        previousGravityAtGrounded = gravity;
        gravityDirection = Direction.mY;
        updateJumpVelocity = true;
		
		animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {

        bool jumpInputDown = Input.GetKeyDown(KeyCode.LeftControl);
        bool jumpInputUp = Input.GetKeyUp(KeyCode.LeftControl);

        //Register a jump entry 
        if (jumpInputDown && grounded && !jump)
        {
            jump = true;
        }
        //Register a jump release (slow down jump)
        slowJump = jumpInputUp && velocity.y > 0;

        //Update gravity main direction. Do it only at gravity changing AND if player is grounded (to let CustomPhysics update currentNormal).
        if (previousGravityAtGrounded != gravity && grounded)
        {
            gravityDirection = GetGravityMainDirection();
            previousGravityAtGrounded = gravity;
        }

        //Register a walk entry. Input depends on the gravity direction.
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");
        bool chooseHInput = gravityDirection == Direction.mY || gravityDirection == Direction.XmY && Mathf.Abs(hInput) >= Math.Abs(vInput) || gravityDirection == Direction.mXmY && Mathf.Abs(hInput) >= Math.Abs(vInput);
        bool chooseMinusHInput = gravityDirection == Direction.Y || gravityDirection == Direction.XY && Mathf.Abs(hInput) >= Math.Abs(vInput) || gravityDirection == Direction.mXY && Mathf.Abs(hInput) >= Math.Abs(vInput);
        bool chooseVInput = gravityDirection == Direction.X || gravityDirection == Direction.XY && Mathf.Abs(vInput) > Math.Abs(hInput) || gravityDirection == Direction.XmY && Mathf.Abs(vInput) > Math.Abs(hInput);
        bool chooseMinusVInput = gravityDirection == Direction.mX || gravityDirection == Direction.mXY && Mathf.Abs(vInput) > Math.Abs(hInput) || gravityDirection == Direction.mXmY && Mathf.Abs(vInput) > Math.Abs(hInput);
        walkInput = 0;
        if (chooseHInput)
        {
            walkInput = hInput;
        }
        else if (chooseMinusHInput)
        {
            walkInput = -hInput;
        }
        else if (chooseVInput)
        {
            walkInput = vInput;
        }
        else if (chooseMinusVInput)
        {
            walkInput = -vInput;
        }
    }

    private Direction GetGravityMainDirection()
    {

        Direction dir = Direction.mY;
        float gravityY = Mathf.Round(gravity.y * 100f) / 100f;
        float gravityX = Mathf.Round(gravity.x * 100f) / 100f;
        if (Mathf.Abs(gravityY) > Mathf.Abs(gravityX))
        {
            dir = gravityY < 0 ? Direction.mY : Direction.Y;
        }
        else if (Mathf.Abs(gravityX) > Mathf.Abs(gravityY))
        {
            dir = gravityX < 0 ? Direction.mX : Direction.X;
        }
        else
        {//Mathf.Abs(gravityY) == Mathf.Abs(gravityX)
            if (gravityY > 0)
            {
                if (gravityX > 0)
                {
                    dir = Direction.XY;
                }
                else
                {
                    dir = Direction.mXY;
                }
            }
            else
            {
                if (gravityX > 0)
                {
                    dir = Direction.XmY;
                }
                else
                {
                    dir = Direction.mXmY;
                }
            }
        }
        return dir;
    }

    protected override Vector2 ComputeVelocity()
    {

        Vector2 targetVelocity = Vector2.zero;

        if (updateJumpVelocity)
        {
            //Jump continuity after gravity changing
            targetVelocity.y = (Quaternion.Euler(0f, 0f, gravityDiff) * velocity).y;
            updateJumpVelocity = false;
        }
        else
        {
            //Jump
            if (jump)
            {
                targetVelocity.y = jumpTakeOffSpeed;
                jump = false;
            }
            else if (slowJump)
            {
                targetVelocity.y = velocity.y * 0.5f;
            }
            else
            {
                targetVelocity.y = velocity.y;
            }
        }

        //Walk
        if (walkInput != 0)
        {
            targetVelocity.x = walkInput * maxSpeed;
			animator.SetBool("IsWalking", true);
        }
        else
        {
            targetVelocity.x = velocity.x;
			animator.SetBool("IsWalking", false);
        }
        bool flipSprite = (spriteRenderer.flipX ? (walkInput > 0.01f) : (walkInput < 0.01f));
        if (flipSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        return targetVelocity;

    }

    public override void SetGravity(Vector2 newGravity)
    {

        gravityDiff = Vector2.SignedAngle(newGravity, gravity);
        base.SetGravity(newGravity);
        updateJumpVelocity = true;
    }
}
