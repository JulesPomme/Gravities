using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPhysics : MonoBehaviour {

    public float maxGroundSlopeAngle = 45f;
    public float gravityModifier = 1f;
    public Vector2 defaultGravity = new Vector2(0, -1);

    protected bool grounded;
    protected Rigidbody2D rb2d;
    /// <summary>
    /// The current velocity of this object. Express it as if gravity and jump were always along y axis and along ground movement along x axis.
    /// </summary>
    protected Vector2 velocity;
    /// <summary>
    /// The target velocity of this object (on next FixedUpdate). Express it as if gravity and jump were always along y axis and along ground movement along x axis.
    /// </summary>
    protected Vector2 targetVelocity;
    protected Vector2 gravity;
    protected Vector2 groundNormal;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    void OnEnable() {
        rb2d = GetComponent<Rigidbody2D>();
        gravity = defaultGravity;
    }

    void Start() {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    private void Update() {

        targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity() {
    }

    public void SwitchGravity(Vector2 newGravity) {
        gravity = newGravity;
    }

    void FixedUpdate() {

        //Apply standard gravity to velocity
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;

        ////Apply target velocity
        //velocity.x = targetVelocity.x;

        //Convert velocity to a position shift
        Vector2 deltaPosition = velocity * Time.deltaTime;

        grounded = false;

        ////Movement along ground

        //Vector2 directionAlongGround = new Vector2(groundNormal.y, -groundNormal.x);
        //Vector2 movementAlongGround = directionAlongGround * deltaPosition.x;
        //Move(movementAlongGround, false);

        //Movement along gravity
        Vector2 directionAlongGravity = -gravity;
        Vector2 movementAlongGravity = directionAlongGravity * deltaPosition.y;
        Move(movementAlongGravity, true);
    }

    void Move(Vector2 movement, bool alongGravityMovement) {
        float distance = movement.magnitude;

        if (distance > minMoveDistance) {
            int count = rb2d.Cast(movement, contactFilter, hitBuffer, distance + shellRadius);

            for (int i = 0; i < count; i++) {
                Vector2 currentNormal = hitBuffer[i].normal;
                float slopeAngle = Vector2.Angle(currentNormal, -gravity);
                if (slopeAngle < maxGroundSlopeAngle) {
                    grounded = true;
                    if (alongGravityMovement) {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);
                Debug.Log("currentNormal = " + currentNormal + ", velocity = " + velocity + ", projection = " + projection);
                if (projection < 0) {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBuffer[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }

        rb2d.position = rb2d.position + movement.normalized * distance;
    }

    public bool IsGrounded() {
        return grounded;
    }
}
