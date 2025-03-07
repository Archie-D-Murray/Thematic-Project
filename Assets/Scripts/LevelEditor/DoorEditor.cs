using System.Collections.Generic;
using System.Linq;

using Data;

using UI;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Utilities;

namespace LevelEditor {
    public class DoorEditor : MonoBehaviour, ISerialize {
        [SerializeField] private List<Door> _doors = new List<Door>();
        [SerializeField] private GameObject _doorPrefab;
        [SerializeField] private LayerMask _doorMask;

        [SerializeField] private Transform _move;
        [SerializeField] private Door _selected = null;

        [SerializeField] private Button _spawnButton;

        private void Start() {
            foreach (Door door in FindObjectsOfType<Door>()) {
                _doors.Add(door);
            }
            _spawnButton = GetComponent<Button>();
            _spawnButton.onClick.AddListener(OnSpawn);
            _spawnButton.GetComponentsInChildren<Image>().First(image => image.transform != transform).sprite =
                _doorPrefab.GetComponent<SpriteRenderer>().sprite; // Auto update sprite for button when we make asset
            _doorMask = 1 << LayerMask.NameToLayer("Door");
        }

        private void OnSpawn() {
            _doors.Add(Instantiate(_doorPrefab, Helpers.Instance.TileMapMousePosition, Quaternion.identity).GetComponent<Door>());
            _selected = _doors.Last();
            _move = _selected.transform;
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape) || (!UIManager.Instance.IsHovered() && Input.GetMouseButtonDown(0))) {
                _selected = null;
                _move = null;
                EventSystem.current.SetSelectedGameObject(null); // Space can press button ffs
                return;
            }

            if (!_selected && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Q))) { // For laptop people :)
                RaycastHit2D hit = Physics2D.Raycast(Helpers.Instance.TileMapMousePosition, Vector2.down, 2.0f, _doorMask);
                if (hit && hit.transform.TryGetComponent(out Door door)) {
                    _selected = door;
                    _move = door.transform;
                }
            }

            if (!_selected && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.X))) {
                RaycastHit2D hit = Physics2D.Raycast(Helpers.Instance.TileMapMousePosition, Vector2.down, 2.0f, _doorMask);
                if (hit && hit.transform.TryGetComponent(out Door door)) {
                    _doors.Remove(door);
                    Destroy(door.gameObject);
                }
            }

            if (_selected && Input.GetKeyDown(KeyCode.Tab) && !Input.GetKey(KeyCode.LeftShift) && _selected.transform == _move) { // Cycle to next move point
                _move = _selected.Button.transform;
            }
            if (_selected && Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift) && _selected.transform != _move) {
                _move = _selected.transform;
            }
            if (_selected && _move) {
                _move.position = Helpers.Instance.TileMapMousePosition;
            }
        }

        public void OnLoad(LevelData data) {
            Door[] doors = FindObjectsOfType<Door>();
            foreach (Door door in _doors) {
                Destroy(door);
            }
            _doors.Clear();
            foreach (DoorData doorData in data.DoorData) {
                Instantiate(_doorPrefab).GetComponent<Door>().LoadSaveData(doorData);
            }
        }

        public void OnSave(ref LevelData data) {
            foreach (Door door in _doors) {
                data.DoorData.Add(door.ToSaveData());
            }
        }
    }
}