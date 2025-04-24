using UnityEngine;

using System.Collections.Generic;

using LevelEditor;
using Entity.Player;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Tags.UI;

namespace Game {

    [Serializable]
    public class TutorialUI {
        public Image Panel;
        public Image Readout;
        public TMP_Text ReadoutText;

        public TutorialUI(GameObject instance, Tutorial tutorial, TutorialManager manager) {
            Panel = instance.GetComponent<Image>();
            Readout = instance.GetComponentsInChildren<Image>().First(image => image.gameObject.HasComponent<ReadoutTag>());
            foreach (TMP_Text text in instance.GetComponentsInChildren<TMP_Text>()) {
                if (text.gameObject.HasComponent<ReadoutTag>()) {
                    ReadoutText = text;
                    int steps = tutorial.GetSteps(manager);
                    text.text = steps > 0 ? $"0 / {steps}" : "";
                } else {
                    text.text = tutorial.Task;
                }
            }
        }
    }

    public class TutorialManager : MonoBehaviour {
        public int MinObstacleTypes = 3;
        public List<ObstacleType> PlacedObstacles;
        public bool HasDestroyedObstacle;
        public bool HasPickedUpObstacle;
        public List<TilemapEditor.TilemapType> PlacedTiles;
        public int MinTileTypes = 2;
        public bool KilledEnemy;

        [SerializeField] private Tutorial[] _tutorials;
        private Dictionary<TutorialType, Tutorial> _lookup;
        private Dictionary<TutorialType, TutorialUI> _uiPanels;
        [SerializeField] private CanvasGroup _canvas;
        [SerializeField] private Transform _scroll;

        PlayerController _playerController;
        ObstacleEditor _obstacleEditor;
        TilemapEditor _tilemapEditor;

        private void Start() {
            if (!_canvas) {
                _canvas = GetComponent<CanvasGroup>();
            }
            foreach (Tutorial tutorial in _tutorials) {
                _lookup.Add(tutorial.Type, tutorial);
                _uiPanels.Add(tutorial.Type, new TutorialUI(Instantiate(tutorial.Panel, _scroll), tutorial, this));
            }
            _playerController = FindFirstObjectByType<PlayerController>();
            _obstacleEditor = FindFirstObjectByType<ObstacleEditor>();
            _tilemapEditor = FindFirstObjectByType<TilemapEditor>();

            _playerController.OnKill += EnemyKill;
            _tilemapEditor.OnTilePlace += UpdatePlacedTiles;
            _obstacleEditor.OnPlace += UpdatePlacedObstacles;
            _obstacleEditor.OnDestroy += RemoveObstacle;
            _obstacleEditor.OnPickup += PickupObstacle;
        }

        private void UpdatePlacedTiles(TilemapEditor.TilemapType type) {
            if (!PlacedTiles.Contains(type)) {
                PlacedTiles.Add(type);
                TryComplete(TutorialType.PlaceTile);
            }
        }

        private void UpdatePlacedObstacles(ObstacleType type) {
            if (!PlacedObstacles.Contains(type)) {
                PlacedObstacles.Add(type);
                TryComplete(TutorialType.PlaceObstacle);
            }
        }

        private void RemoveObstacle() {
            HasDestroyedObstacle = true;
            TryComplete(TutorialType.RemoveObstacle);
        }

        private void PickupObstacle() {
            HasPickedUpObstacle = true;
            TryComplete(TutorialType.PickupObstacle);
        }

        private void EnemyKill() {
            KilledEnemy = true;
            TryComplete(TutorialType.Attack);
        }

        private void TryComplete(TutorialType type) {
            if (_lookup.TryGetValue(type, out Tutorial tutorial) && tutorial.CheckComplete(this, _uiPanels[type])) {
                switch (type) {
                    case TutorialType.Attack:
                        _playerController.OnKill -= EnemyKill;
                        break;
                    case TutorialType.PlaceTile:
                        _tilemapEditor.OnTilePlace -= UpdatePlacedTiles;
                        break;
                    case TutorialType.PlaceObstacle:
                        _obstacleEditor.OnPlace -= UpdatePlacedObstacles;
                        break;
                    case TutorialType.RemoveObstacle:
                        _obstacleEditor.OnDestroy -= RemoveObstacle;
                        break;
                    case TutorialType.PickupObstacle:
                        _obstacleEditor.OnPickup -= PickupObstacle;
                        break;
                    default:
                        break;
                }
            }
        }

        public void Open() {
            if (_canvas.alpha == 0) {
                _canvas.FadeCanvas(2.0f, false, this);
                return;
            }
        }

        public void Close() {
            if (_canvas.alpha == 1) {
                _canvas.FadeCanvas(2.0f, true, this);
                return;
            }
        }
    }
}