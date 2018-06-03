using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPhysics : MonoBehaviour {

    //TODO 
    // - changement de gravité pendant un saut => adapter le sens de la vélocité
    // - en arrière après un saut (à la Mario) ?
    // - Bug : Pourquoi ne glisse-t-on plus sur les plateformes très inclinées ? (impossible de sauter également)

    public float maxGroundSlopeAngle = 45f;
    public float gravityModifier = 1f;
    public Vector2 defaultGravity = new Vector2(0, -1);

    protected bool grounded;
    protected Rigidbody2D rb2d;
    /// <summary>
    /// The current velocity of this object. The x value is the velocity along ground, the y value is the velocity along gravity.
    /// </summary>
    protected Vector2 velocity;
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

    protected virtual Vector2 ComputeVelocity() {

        return Vector2.zero;
    }

    public virtual void SetGravity(Vector2 newGravity) {
        gravity = newGravity;
    }

    void FixedUpdate() {

        //get computed velocity only for inherited classes
        if (GetType() != typeof(CustomPhysics)) {
            velocity = ComputeVelocity();
        }

        //Apply gravity to velocity
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;

        //Convert velocity to a position shift
        Vector2 deltaPosition = velocity * Time.deltaTime;

        grounded = false;

        //Movement along ground
        Vector2 directionAlongGround = new Vector2(groundNormal.y, -groundNormal.x);
        Vector2 movementAlongGround = directionAlongGround * deltaPosition.x;
        Move(movementAlongGround, false);

        //Movement along gravity
        Vector2 antiGravity = -gravity;
        Vector2 movementAlongGravity = antiGravity * deltaPosition.y;
        Move(movementAlongGravity, true);

        //Apply friction if grounded
        ApplyGroundFriction();
    }

    void Move(Vector2 movement, bool alongGravityMovement) {
        float distance = movement.magnitude;

        if (distance > minMoveDistance) {
            int count = rb2d.Cast(movement, contactFilter, hitBuffer, distance + shellRadius);
            float gravityAngle = Vector2.SignedAngle(gravity, Physics2D.gravity);
            for (int i = 0; i < count; i++) {
                Vector2 currentNormal = hitBuffer[i].normal;
                Vector2 rotatedNormal = Quaternion.Euler(0f, 0f, gravityAngle) * currentNormal;
                float slopeAngle = Vector2.Angle(currentNormal, -gravity);

                if (slopeAngle < maxGroundSlopeAngle) {
                    grounded = true;
                    if (alongGravityMovement) {
                        groundNormal = currentNormal;
                        rotatedNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, rotatedNormal);
                if (projection < 0) {
                    //If object hits a wall or a ceil, remove velocity in its direction.
                    velocity = velocity - projection * rotatedNormal;
                }

                float modifiedDistance = hitBuffer[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }

        rb2d.position = rb2d.position + movement.normalized * distance;
    }

    private void ApplyGroundFriction() {

        if (grounded) {
            if (velocity.x > 0.01f) {
                velocity.x *= 0.9f;
            } else {
                velocity.x = 0f;
            }
        }
    }

    public bool IsGrounded() {
        return grounded;
    }
}
