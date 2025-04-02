using Entity.Player;

using LevelEditor;

using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] private PlayerController _player;
    [SerializeField] private Transform _target;

    [SerializeField] private EditorManager _editorManager;
    [SerializeField] private float _cameraSpeed = 5.0f;

    [SerializeField] private bool _inputControl = true;

    private void Start() {
        _player = FindObjectOfType<PlayerController>();
        _editorManager = FindObjectOfType<EditorManager>();
        _editorManager.OnPlay += OnPlay;
        _target = transform.GetChild(0);
        _target.position = _player.transform.position;
    }

    public void LateUpdate() {
        if (_inputControl) {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            _target.position += _cameraSpeed * Time.deltaTime * (Vector3)input.normalized;
        } else {
            _target.position = _player.transform.position;
        }
    }

    public void OnPlay(PlayState state) {
        _player.OnPlay(state);
        _inputControl = state != PlayState.Begin;
    }
}