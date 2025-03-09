using System.Collections.Generic;
using System.Linq;

using Data;

using UI;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Utilities;

using Tags.UI;
using TMPro;

namespace LevelEditor {
    public class ObstacleEditor : MonoBehaviour, ISerialize {
        [SerializeField] private List<Placeable> _placeables = new List<Placeable>();
        [SerializeField] private ObstacleData[] _obstacleData;
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private GameObject _obstaclePanelPrefab;
        [SerializeField] private GameObject _indicatorPrefab;

        [SerializeField] private Transform _move;
        [SerializeField] private Placeable _selected = null;

        [SerializeField] private int _index = 0;

        private Dictionary<ObstacleType, ObstacleData> _obstacleLookup = new Dictionary<ObstacleType, ObstacleData>();
        private SpriteRenderer _indicator;

        private void Start() {
            foreach (ObstacleData data in _obstacleData) {
                _obstacleLookup.Add(data.Obstacle, data);
                GameObject panel = Instantiate(_obstaclePanelPrefab, transform);
                panel.GetComponentsInChildren<Image>().First(image => image.gameObject.HasComponent<IconTag>()).sprite = data.Sprite;
                panel.GetComponentsInChildren<TMP_Text>().First(text => text.gameObject.HasComponent<ReadoutTag>()).text = $"[{data.Key}]";
            }
            foreach (Placeable placeable in FindObjectsOfType<Placeable>()) {
                _placeables.Add(placeable);
            }
            _obstacleMask = 1 << LayerMask.NameToLayer("Obstacle");
            _indicator = Instantiate(_indicatorPrefab).GetComponent<SpriteRenderer>();
            _indicator.gameObject.SetActive(false);
        }

        private void OnSpawn(ObstacleData data) {
            _placeables.Add(Instantiate(data.Prefab, Helpers.Instance.TileMapMousePosition, Quaternion.identity).GetComponent<Placeable>());
            _selected = _placeables.Last();
            _selected.StartPlacement();
            _selected.InitReferences();
            _move = _selected.GetInitial();
            _index = 0;
            _indicator.gameObject.SetActive(true);
        }

        private void Update() {
            foreach (ObstacleData data in _obstacleData) {
                if (Input.GetKeyDown(data.Key)) {
                    OnSpawn(data);
                    return;
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape) || (!UIManager.Instance.IsHovered() && Input.GetMouseButtonDown(0))) {
                _selected.FinishPlacement();
                _selected = null;
                _move = null;
                EventSystem.current.SetSelectedGameObject(null); // Space can press button ffs
                _indicator.gameObject.SetActive(false);
                return;
            }

            if (!_selected && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Q))) { // For laptop people :)
                RaycastHit2D hit = Physics2D.Raycast(Helpers.Instance.TileMapMousePosition, Vector2.down, 2.0f, _obstacleMask);
                if (hit && hit.transform.TryGetComponent(out Placeable placeable)) {
                    _selected = placeable;
                    _selected.StartPlacement();
                    _move = placeable.GetInitial();
                    _indicator.gameObject.SetActive(true);
                    _index = 0;
                }
            }

            if (!_selected && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.X))) {
                RaycastHit2D hit = Physics2D.Raycast(Helpers.Instance.TileMapMousePosition, Vector2.down, 2.0f, _obstacleMask);
                if (hit && hit.transform.TryGetComponent(out Placeable placeable)) {
                    _placeables.Remove(placeable);
                    Destroy(placeable.gameObject);
                }
            }

            if (_selected && Input.GetKeyDown(KeyCode.Tab) && !Input.GetKey(KeyCode.LeftShift)) { // Cycle to next move point
                _move = _selected.GetNext(ref _index);
            }
            if (_selected && Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift)) {
                _move = _selected.GetPrev(ref _index);
            }
            if (_selected && _move) {
                _move.position = Helpers.Instance.TileMapMousePosition;
                _indicator.transform.position = Helpers.Instance.TileMapMousePosition;
            }
        }

        public void OnLoad(LevelData data) {
            Placeable[] placeables = FindObjectsOfType<Placeable>();
            foreach (Placeable placeable in placeables) {
                Destroy(placeable.gameObject);
            }
            _placeables.Clear();
            foreach (DoorData doorData in data.DoorData) {
                Door door = Instantiate(_obstacleLookup[ObstacleType.Door].Prefab).GetComponent<Door>();
                _placeables.Add(door);
                door.LoadSaveData(doorData);
            }
            foreach (PlatformData platformData in data.PlatformData) {
                MovingPlatform platform = Instantiate(_obstacleLookup[ObstacleType.Platform].Prefab).GetComponent<MovingPlatform>();
                platform.LoadSaveData(platformData);
            }
            // TODO: Handle other obstacle types
        }

        public void OnSave(ref LevelData data) {
            foreach (Placeable placeable in _placeables) {
                if (placeable is Door) {
                    Door door = placeable as Door;
                    data.DoorData.Add(door.ToSaveData());
                } else if (placeable is MovingPlatform) {
                    MovingPlatform platform = placeable as MovingPlatform;
                    data.PlatformData.Add(platform.ToSaveData());
                }
                // TODO: Handle other obstacle types
            }
        }
    }
}