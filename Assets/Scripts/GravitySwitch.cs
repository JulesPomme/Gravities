using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySwitch : MonoBehaviour {

    private void OnTriggerStay2D(Collider2D collision) {

        if (collision.gameObject.tag == "Player") {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (!playerController.IsGrounded())
                playerController.SetGravity(-transform.up);
        }
    }
}
