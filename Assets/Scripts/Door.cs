using System.Collections;
using System.Collections.Generic;

using Data;

using LevelEditor;

using UnityEngine;

public class Door : Placeable {
    [SerializeField] bool open;
    SpriteRenderer doorSprite;
    BoxCollider2D doorCollider;
    private DoorButton _button;

    public DoorButton Button {
        get {
            if (!_button) {
                _button = GetComponentInChildren<DoorButton>();
            }
            return _button;
        }
    }

    protected override Transform[] GetMoveables() {
        return new Transform[] { transform, Button.transform };
    }

    private void Awake() {
        open = false;
        _button = GetComponentInChildren<DoorButton>();
        doorSprite = GetComponentInChildren<SpriteRenderer>();
        doorCollider = GetComponentInChildren<BoxCollider2D>();
        _moveables = GetMoveables();
    }

    public void Toggle() {
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

    public DoorData ToSaveData() {
        return new DoorData(transform.position, _button.transform.position, open);
    }

    public void LoadSaveData(DoorData data) {
        transform.position = data.DoorPosition;
        _button.transform.position = data.ButtonPosition;
        if (data.Open != open) {
            Toggle();
        }
    }

    // TO DO - door open and closed sprites (animator?)

}