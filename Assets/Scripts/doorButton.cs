using System;

using Entity.Player;

using UnityEngine;

public class DoorButton : MonoBehaviour {

    Door door;
    Boolean canInteract;

    // Start is called before the first frame update
    void Start() {
        canInteract = false;
        door = transform.parent.GetComponent<Door>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E) && canInteract) {
            door.Toggle();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.HasComponent<PlayerController>()) {
            canInteract = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.HasComponent<PlayerController>()) {
            canInteract = false;
        }
    }
}