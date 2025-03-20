using System.Collections.Generic;
using System.Linq;

using Data;

using UI;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Utilities;

using Tags.UI;
using Tags;
using Tags.Obstacle;
using UnityEngineInternal;

namespace LevelEditor {
    public class ObstacleEditor : MonoBehaviour, ISerialize {
        [SerializeField] private List<Placeable> _placeables = new List<Placeable>();
        [SerializeField] private ObstacleData[] _obstacleData;
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private GameObject _obstaclePanelPrefab;

        [SerializeField] private Transform _move;
        [SerializeField] private Placeable _selected = null;

        [SerializeField] private int _index = 0;
        [SerializeField] private KeyCode _hotkey = KeyCode.F11;
        [SerializeField] private CanvasGroup _canvas;

        private Dictionary<ObstacleType, ObstacleData> _obstacleLookup = new Dictionary<ObstacleType, ObstacleData>();

        private void Start() {
            foreach (ObstacleData data in _obstacleData) {
                _obstacleLookup.Add(data.Obstacle, data);
                GameObject panel = Instantiate(_obstaclePanelPrefab, transform);
                foreach (Image image in panel.GetComponentsInChildren<Image>()) {
                    if (image.transform == panel.transform) { continue; }
                    // TODO: This is not very robust - may need to be changed later
                    if (image.gameObject.HasComponent<IconTag>()) {
                        image.sprite = data.Sprite;
                    } else {
                        image.sprite = data.KeySprite;
                    }
                }
            }
            foreach (Placeable placeable in FindObjectsOfType<Placeable>()) {
                _placeables.Add(placeable);
            }
            _obstacleMask = 1 << LayerMask.NameToLayer("Obstacle") | 1 << LayerMask.NameToLayer("Enemy");
            _canvas = GetComponent<CanvasGroup>();
            _canvas.FadeCanvas(0.01f, true, this);

        }

        private void OnSpawn(ObstacleData data) {
            _placeables.Add(Instantiate(data.Prefab, Helpers.Instance.TileMapMousePosition, Quaternion.identity).GetComponent<Placeable>());
            _selected = _placeables.Last();
            _selected.StartPlacement();
            _selected.InitReferences();
            _move = _selected.GetInitial();
            _index = 0;
        }

        private void Update() {
            if (_canvas.alpha != 1.0f) { return; }
            foreach (ObstacleData data in _obstacleData) {
                if (Input.GetKeyDown(data.Key)) {
                    OnSpawn(data);
                    return;
                }
            }
            if (_selected && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))) {
                _selected.FinishPlacement();
                _selected = null;
                _move = null;
                EventSystem.current.SetSelectedGameObject(null); // Space can press button ffs
                return;
            }

            if (!_selected && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Q))) { // For laptop people :)
                RaycastHit2D hit = Physics2D.Raycast(Helpers.Instance.TileMapMousePosition, Vector2.down, 2.0f, _obstacleMask);
                if (hit) {
                    Placeable placeable = null;
                    if (hit.collider.gameObject.HasComponent<ColliderOnChild>()) {
                        hit.transform.parent.TryGetComponent(out placeable);
                    } else {
                        hit.transform.TryGetComponent(out placeable);
                    }
                    if (placeable) {
                        _selected = placeable;
                        _selected.InitReferences();
                        _selected.StartPlacement();
                        _move = placeable.GetInitial();
                        _index = 0;
                    }
                }
            }

            if (!_selected && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.X))) {
                RaycastHit2D hit = Physics2D.Raycast(Helpers.Instance.TileMapMousePosition, Vector2.down, 2.0f, _obstacleMask);
                if (hit) {
                    Placeable placeable = null;
                    if (hit.collider.gameObject.HasComponent<ColliderOnChild>()) {
                        hit.transform.parent.TryGetComponent(out placeable);
                    } else {
                        hit.transform.TryGetComponent(out placeable);
                    }
                    if (placeable) {
                        _placeables.Remove(placeable);
                        Destroy(placeable.gameObject);
                    }
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
                _placeables.Add(platform);
                platform.LoadSaveData(platformData);
            }
            foreach (PlatformData platformData in data.DeathPlatformData) {
                MovingPlatform platform = Instantiate(_obstacleLookup[ObstacleType.DeathPlatform].Prefab).GetComponent<MovingPlatform>();
                _placeables.Add(platform);
                platform.LoadSaveData(platformData);
            }
            foreach (PatrolEnemyData patrolEnemy in data.PatrolEnemies) {
                PatrolEnemy patrol = Instantiate(_obstacleLookup[ObstacleType.PatrolEnemy].Prefab).GetComponentInChildren<PatrolEnemy>();
                _placeables.Add(patrol);
                patrol.LoadPatrolEnemyData(patrolEnemy);
            }
            foreach (FlyingEnemyData flyingEnemy in data.FlyingEnemies) {
                FlyingEnemy flying = Instantiate(_obstacleLookup[ObstacleType.FlyingEnemy].Prefab).GetComponent<FlyingEnemy>();
                _placeables.Add(flying);
                flying.LoadFlyingEnemyData(flyingEnemy);
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
                    if (placeable.gameObject.HasComponent<KillZone>()) {
                        data.DeathPlatformData.Add(platform.ToSaveData());
                    } else {
                        data.PlatformData.Add(platform.ToSaveData());
                    }
                } else if (placeable is PatrolEnemy) {
                    PatrolEnemy patrolEnemy = placeable as PatrolEnemy;
                    data.PatrolEnemies.Add(patrolEnemy.ToPatrolEnemyData());
                } else if (placeable is FlyingEnemy) {
                    FlyingEnemy flyingEnemy = placeable as FlyingEnemy;
                    data.FlyingEnemies.Add(flyingEnemy.ToFlyingEnemyData());
                }
                // TODO: Handle other obstacle types
            }
        }

        public void Open() {
            if (_canvas.alpha == 0) {
                _canvas.FadeCanvas(0.5f, false, this);
                return;
            }
        }

        public void Close() {
            if (_canvas.alpha == 1) {
                _canvas.FadeCanvas(0.5f, true, this);
                return;
            }
        }
    }
}