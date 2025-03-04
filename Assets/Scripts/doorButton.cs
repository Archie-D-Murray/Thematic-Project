using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class doorButton : MonoBehaviour
{

    Door door;
    Boolean canInteract;

    // Start is called before the first frame update
    void Start()
    {
        canInteract = false;
        door = transform.parent.GetComponent<Door>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E) && canInteract)  {
            door.toggle();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            canInteract = true;        
            }

    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            canInteract = false;
        }
    }
}
