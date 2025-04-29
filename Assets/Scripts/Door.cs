using Data;

using LevelEditor;

using UnityEngine;

public class Door : Placeable {
    [SerializeField] bool _open;
    [SerializeField] private Sprite _openDoorSprite;
    [SerializeField] private Sprite _closedDoorSprite;
    private SpriteRenderer _doorSprite;
    private BoxCollider2D _doorCollider;
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
        _open = false;
        _button = GetComponentInChildren<DoorButton>();
        _doorSprite = GetComponentInChildren<SpriteRenderer>();
        _doorCollider = GetComponentInChildren<BoxCollider2D>();
    }

    private void Start() {
        InitReferences();
    }

    public void Toggle() {
        _open = !_open;
    }

    private void Update() {
        if (_open) {
            _doorSprite.sprite = _closedDoorSprite;
            _doorCollider.enabled = false;
        } else {
            _doorSprite.sprite = _openDoorSprite;
            _doorCollider.enabled = true;
        }
    }

    public DoorData ToSaveData() {
        return new DoorData(_initialPosition, _button.transform.position, _open);
    }

    public void LoadSaveData(DoorData data) {
        transform.position = data.DoorPosition;
        _button.transform.position = data.ButtonPosition;
        if (data.Open != _open) {
            Toggle();
        }
    }

    // TO DO - door open and closed sprites (animator?)

}