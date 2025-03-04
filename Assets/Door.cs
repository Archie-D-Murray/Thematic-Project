using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] bool open;
    SpriteRenderer doorSprite;
    BoxCollider2D doorCollider;

    private void Start() {
        open = false;
        doorSprite = GetComponentInChildren<SpriteRenderer>();
        doorCollider = GetComponentInChildren<BoxCollider2D>();
    }

    public void toggle() {
        open = !open;
    }

    private void Update() {
        if (open) {
            doorSprite.enabled = false;
            doorCollider.enabled = false;
        } else {
            doorSprite.enabled = true;
            doorCollider.enabled = true;
        }
    }

    // TO DO - door open and closed sprites (animator?)

}