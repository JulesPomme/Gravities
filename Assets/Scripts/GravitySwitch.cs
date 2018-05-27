using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySwitch : MonoBehaviour {

    private Dictionary<GameObject, bool> registeredEntries;

    private void Start() {
        registeredEntries = new Dictionary<GameObject, bool>();
    }

    private void OnTriggerStay2D(Collider2D collision) {

        CustomPhysics customPhysics = collision.gameObject.GetComponent<CustomPhysics>();
        if (customPhysics != null) {
            if (!registeredEntries.ContainsKey(customPhysics.gameObject))
                registeredEntries[customPhysics.gameObject] = false;
            if (!customPhysics.IsGrounded() && !registeredEntries[customPhysics.gameObject]) {
                customPhysics.SetGravity(-transform.up);
                registeredEntries[customPhysics.gameObject] = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {

        CustomPhysics customPhysics = collision.gameObject.GetComponent<CustomPhysics>();
        if (customPhysics != null && registeredEntries[customPhysics.gameObject]) {
            registeredEntries[customPhysics.gameObject] = false;
        }
    }
}
