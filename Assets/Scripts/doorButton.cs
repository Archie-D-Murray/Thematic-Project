using System;

using Entity.Player;

using UnityEngine;

public class DoorButton : MonoBehaviour {

    Door door;
    Boolean canInteract;
    [SerializeField] private Sprite leverOn;
    [SerializeField] private Sprite leverOff;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start() {
        canInteract = false;
        door = transform.parent.GetComponent<Door>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E) && canInteract) {
            door.Toggle();
            if(spriteRenderer.sprite == leverOn) {
                spriteRenderer.sprite = leverOff;
            } else {
                spriteRenderer.sprite = leverOn;
            }
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