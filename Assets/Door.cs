using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] bool open;
    Sprite openSprite;
    Sprite closeSprite;

    private void Start() {
        open = false;
    }

    // TO DO - door open and closed sprites (animator?)

}
