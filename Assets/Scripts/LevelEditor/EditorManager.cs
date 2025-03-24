using UnityEngine;
using UnityEngine.UI;

using System.Linq;
using System;

namespace LevelEditor {

    public enum EditorState { None, Obstacle, Tilemap, Save }

    public class EditorManager : MonoBehaviour {

        [SerializeField] private Button _save;
        [SerializeField] private Button _tilemap;
        [SerializeField] private Button _obstacle;

        [SerializeField] private ObstacleEditor _obstacleEditor;
        [SerializeField] private TilemapEditor _tilemapEditor;
        [SerializeField] private SaveUI _saveUI;
        [SerializeField] private EditorState _state;

        public EditorState State => _state;

        private void Start() {
            _save.onClick.AddListener(EnableSave);
            _tilemap.onClick.AddListener(EnableTilemap);
            _obstacle.onClick.AddListener(EnableObstacle);
        }

        private void EnableTilemap() {
            if (_state == EditorState.Tilemap) {
                _tilemapEditor.Close();
                _state = EditorState.None;
            } else {
                _tilemapEditor.Open();
                _state = EditorState.Tilemap;
            }
            _saveUI.Close();
            _obstacleEditor.Close();
        }

        private void EnableObstacle() {
            if (_state == EditorState.Obstacle) {
                _obstacleEditor.Close();
                _state = EditorState.None;
            } else {
                _obstacleEditor.Open();
                _state = EditorState.Obstacle;
            }
            if (_obstacleEditor.Alpha != 0.0f) {
                _obstacleEditor.Close();
            } else if (_obstacleEditor.Alpha != 1.0f) {
                _obstacleEditor.Open();
            }
            _tilemapEditor.Close();
            _saveUI.Close();
        }

        private void EnableSave() {
            if (_state == EditorState.Save) {
                _saveUI.Close();
                _state = EditorState.None;
            } else {
                _saveUI.Open();
                _state = EditorState.Save;
            }
            _tilemapEditor.Close();
            _obstacleEditor.Close();
        }
    }
}