using System.Collections.Generic;
using System.Linq;

using Data;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Utilities;

using Tags.UI;
using Tags;
using Tags.Obstacle;
using System;

namespace LevelEditor {
    [DefaultExecutionOrder(-99)]
    public class ObstacleEditor : MonoBehaviour, ISerialize {
        [SerializeField] private List<Placeable> _placeables = new List<Placeable>();
        [SerializeField] private ObstacleData[] _obstacleData;
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private GameObject _obstaclePanelPrefab;

        [SerializeField] private Transform _move;
        [SerializeField] private Placeable _selected = null;

        [SerializeField] private int _index = 0;
        [SerializeField] private CanvasGroup _canvas;
        [SerializeField] private bool _hasSpawnPoint = false;
        [SerializeField] private float _gracePeriod = 0.75f;

        public Action<ObstacleType> OnPlace;
        public Action OnDestroy;
        public Action OnPickup;
        public Action OnCycleMoveable;

        private Dictionary<ObstacleType, ObstacleData> _obstacleLookup = new Dictionary<ObstacleType, ObstacleData>();
        private CountDownTimer _graceTimer = new CountDownTimer(0.0f);
        public float Alpha => _canvas.alpha;
        public bool HasSpawnPoint => _hasSpawnPoint;
        public Action<bool> UpdateSpawnPoint;

        private void Awake() {
            foreach (ObstacleData data in _obstacleData) {
                _obstacleLookup.Add(data.Obstacle, data);
                GameObject panel = Instantiate(_obstaclePanelPrefab, transform);
                panel.GetComponentInChildren<Button>().onClick.AddListener(() => OnSpawn(_obstacleLookup[data.Obstacle]));
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
                if (placeable is SpawnPoint) {
                    UpdateSpawnPoint?.Invoke(_hasSpawnPoint);
                    _hasSpawnPoint = true;
                }
            }
            _obstacleMask = 1 << LayerMask.NameToLayer("Obstacle") | 1 << LayerMask.NameToLayer("Enemy");
            _canvas = GetComponent<CanvasGroup>();
            _canvas.FadeCanvas(100.0f, true, this);
            EditorManager.Instance.OnPlay += OnPlay;
        }

        private void OnPlay(PlayState state) {
            if (_selected) {
                _selected.FinishPlacement();
                _selected = null;
                _move = null;
                EventSystem.current.SetSelectedGameObject(null); // Space can press button ffs
            }
            foreach (Placeable placeable in _placeables) {
                placeable.OnPlay(state);
            }
        }

        private void OnSpawn(ObstacleData data) {
            _graceTimer.Reset(_gracePeriod);
            _placeables.Add(Instantiate(data.Prefab, Helpers.Instance.TileMapMousePosition, Quaternion.identity).GetComponentInChildren<Placeable>());
            if (data.Obstacle == ObstacleType.SpawnPoint) {
                _hasSpawnPoint = true;
                UpdateSpawnPoint?.Invoke(_hasSpawnPoint);
            }
            _selected = _placeables.Last();
            _selected.InitReferences();
            _selected.StartPlacement();
            _move = _selected.GetInitial();
            OnPlace?.Invoke(data.Obstacle);
            _index = 0;
        }

        private void Update() {
            if (_canvas.alpha != 1.0f) { return; }
            _graceTimer.Update(Time.deltaTime);
            foreach (ObstacleData data in _obstacleData) {
                if (Input.GetKeyDown(data.Key)) {
                    OnSpawn(data);
                    return;
                }
            }
            if (_selected && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0)) && _graceTimer.IsFinished) {
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
                        OnPickup?.Invoke();
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
                        if (placeable is SpawnPoint) {
                            UpdateSpawnPoint?.Invoke(_hasSpawnPoint);
                            _hasSpawnPoint = false;
                        }
                        OnDestroy?.Invoke();
                        _placeables.Remove(placeable);
                        placeable.RemovePlaceable();
                    }
                }
            }

            if (_selected && _selected.MoveableCount > 1 && Input.GetKeyDown(KeyCode.Tab) && !Input.GetKey(KeyCode.LeftShift)) { // Cycle to next move point
                _move = _selected.GetNext(ref _index);
                OnCycleMoveable?.Invoke();
            }
            if (_selected && _selected.MoveableCount > 1 && Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift)) {
                _move = _selected.GetPrev(ref _index);
                OnCycleMoveable?.Invoke();
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
                patrol.LoadSaveData(patrolEnemy);
            }
            foreach (FlyingEnemyData flyingEnemy in data.FlyingEnemies) {
                FlyingEnemy flying = Instantiate(_obstacleLookup[ObstacleType.FlyingEnemy].Prefab).GetComponent<FlyingEnemy>();
                _placeables.Add(flying);
                flying.LoadSaveData(flyingEnemy);
            }
            foreach (LaserData laserData in data.Lasers) {
                Laser laser = Instantiate(_obstacleLookup[ObstacleType.Laser].Prefab).GetComponent<Laser>();
                _placeables.Add(laser);
                laser.LoadSaveData(laserData);
            }
            if (data.SpawnPoint != null) {
                SpawnPoint spawnPoint = Instantiate(_obstacleLookup[ObstacleType.SpawnPoint].Prefab).GetComponent<SpawnPoint>();
                _placeables.Add(spawnPoint);
                spawnPoint.LoadSaveData(data.SpawnPoint);
                _hasSpawnPoint = true;
                UpdateSpawnPoint?.Invoke(_hasSpawnPoint);
            }
            if (data.EndPoint != null) {
                EndPoint endPoint = Instantiate(_obstacleLookup[ObstacleType.EndPoint].Prefab).GetComponent<EndPoint>();
                _placeables.Add(endPoint);
                endPoint.LoadSaveData(data.EndPoint);
            }
            // TODO: Handle other obstacle types
            // // Assets/Scripts/SpawnPoint.cs
        }

        public void OnSave(ref LevelData data) {
            foreach (Placeable placeable in _placeables) {
                if (placeable is Door) {
                    Door door = placeable as Door;
                    data.DoorData.Add(door.ToSaveData());
                } else if (placeable is MovingPlatform) {
                    MovingPlatform platform = placeable as MovingPlatform;
                    if (placeable.GetComponentInChildren<KillZone>()) {
                        data.DeathPlatformData.Add(platform.ToSaveData());
                    } else {
                        data.PlatformData.Add(platform.ToSaveData());
                    }
                } else if (placeable is PatrolEnemy) {
                    PatrolEnemy patrolEnemy = placeable as PatrolEnemy;
                    data.PatrolEnemies.Add(patrolEnemy.ToSaveData());
                } else if (placeable is FlyingEnemy) {
                    FlyingEnemy flyingEnemy = placeable as FlyingEnemy;
                    data.FlyingEnemies.Add(flyingEnemy.ToSaveData());
                } else if (placeable is Laser) {
                    Laser laser = placeable as Laser;
                    data.Lasers.Add(laser.ToSaveData());
                } else if (placeable is SpawnPoint) {
                    data.SpawnPoint = (placeable as SpawnPoint).ToSaveData();
                } else if (placeable is EndPoint) {
                    data.EndPoint = (placeable as EndPoint).ToSaveData();
                }
                // TODO: Handle other obstacle types
            }
        }

        public void Open() {
            if (_canvas.alpha == 0) {
                _canvas.FadeCanvas(Extensions.FadeSpeed, false, this);
                return;
            }
        }

        public void Close() {
            if (_selected) {
                _selected.FinishPlacement();
                _selected = null;
                _move = null;
                EventSystem.current.SetSelectedGameObject(null); // Space can press button ffs
            }
            if (_canvas.alpha == 1) {
                _canvas.FadeCanvas(Extensions.FadeSpeed, true, this);
                return;
            }
        }
    }
}