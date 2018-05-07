using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPhysics : MonoBehaviour {

    public float minGroundNormalY = .65f;
    public float gravityModifier = 1f;

    protected bool grounded;
    protected Vector2 targetVelocity;
    protected Vector2 groundNormal;
    protected Rigidbody2D rb2d;
    protected Vector2 velocity;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected Vector2 gravity;

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    void OnEnable() {
        rb2d = GetComponent<Rigidbody2D>();
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
        gravity = Physics2D.gravity.magnitude * newGravity;
    }

    void FixedUpdate() {

        velocity += gravityModifier * gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;
        Vector2 deltaPosition = velocity * Time.deltaTime;

        grounded = false;

        Vector2 movementAlongGround = new Vector2(groundNormal.y, -groundNormal.x);
        Vector2 movement = movementAlongGround * deltaPosition.x;
        Move(movement, false);

        movement = Vector2.up * deltaPosition.y;
        Move(movement, true);
    }

    void Move(Vector2 move, bool yMovement) {
        float distance = move.magnitude;

        if (distance > minMoveDistance) {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);

            for (int i = 0; i < count; i++) {
                Vector2 currentNormal = hitBuffer[i].normal;
                if (currentNormal.y > minGroundNormalY) {
                    grounded = true;
                    if (yMovement) {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);
                if (projection < 0) {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBuffer[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }


        }

        rb2d.position = rb2d.position + move.normalized * distance;
    }

    public bool IsGrounded() {
        return grounded;
    }

}
