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
    [SerializeField] private Sprite openDoorSprite;
    [SerializeField] private Sprite closedDoorSprite;

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
    }

    private void Start() {
        InitReferences();
    }

    public void Toggle() {
        open = !open;
    }

    private void Update() {
        if (open) {
            doorSprite.sprite = closedDoorSprite;
            doorCollider.enabled = false;
        } else {
            doorSprite.sprite = openDoorSprite;
            doorCollider.enabled = true;
        }
    }

    public DoorData ToSaveData() {
        return new DoorData(_initialPosition, _button.transform.position, open);
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