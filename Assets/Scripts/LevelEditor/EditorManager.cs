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
            _tilemapEditor.Open();
            _saveUI.Close();
            _obstacleEditor.Close();
        }

        private void EnableObstacle() {
            _obstacleEditor.Open();
            _tilemapEditor.Close();
            _saveUI.Close();
        }

        private void EnableSave() {
            _saveUI.Open();
            _tilemapEditor.Close();
            _obstacleEditor.Close();
        }
    }
}