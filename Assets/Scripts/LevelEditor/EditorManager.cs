using UnityEngine;
using UnityEngine.UI;

using System.Linq;
using System;

namespace LevelEditor {
    public class EditorManager : MonoBehaviour {
        [SerializeField] private Button _save;
        [SerializeField] private Button _tilemap;
        [SerializeField] private Button _obstacle;

        [SerializeField] private ObstacleEditor _obstacleEditor;
        [SerializeField] private TilemapEditor _tilemapEditor;
        [SerializeField] private SaveUI _saveUI;

        private void Start() {
            _save.onClick.AddListener(EnableSave);
            _tilemap.onClick.AddListener(EnableTilemap);
            _obstacle.onClick.AddListener(EnableObstacle);
        }

        private void EnableTilemap() {
            if (_tilemapEditor.Alpha != 0.0f) {
                _tilemapEditor.Close();
            } else if (_tilemapEditor.Alpha != 1.0f) {
                _tilemapEditor.Open();
            }
            _saveUI.Close();
            _obstacleEditor.Close();
        }

        private void EnableObstacle() {
            if (_obstacleEditor.Alpha != 0.0f) {
                _obstacleEditor.Close();
            } else if (_obstacleEditor.Alpha != 1.0f) {
                _obstacleEditor.Open();
            }
            _tilemapEditor.Close();
            _saveUI.Close();
        }

        private void EnableSave() {
            if (_saveUI.Alpha != 0.0f) {
                _saveUI.Close();
            } else if (_saveUI.Alpha != 1.0f) {
                _saveUI.Open();
            }
            _tilemapEditor.Close();
            _obstacleEditor.Close();
        }
    }
}