using UnityEngine;
using UnityEngine.UI;

using System.Linq;
using System;
using Utilities;
using UnityEngine.EventSystems;
using Data;
using UI;
using UnityEngine.SceneManagement;

namespace LevelEditor {

    public enum EditorState { None, Obstacle, Tilemap }

    public enum PlayState { Begin, Continue, Exit }

    public class EditorManager : Singleton<EditorManager> {

        [SerializeField] int MainMenuIndex;

        [SerializeField] private Button _save;
        [SerializeField] private Button _tilemap;
        [SerializeField] private Button _obstacle;
        [SerializeField] private Button _play;
        [SerializeField] private Button _continue;
        [SerializeField] private Button _exit;
        [SerializeField] private Button _return;

        [SerializeField] private CanvasGroup _editorButtons;

        [SerializeField] private ObstacleEditor _obstacleEditor;
        [SerializeField] private TilemapEditor _tilemapEditor;
        [SerializeField] private EditorState _state;

        [SerializeField] private KeyCode _tilemapHotkey = KeyCode.F1;
        [SerializeField] private KeyCode _obstacleHotkey = KeyCode.F2;
        [SerializeField] private KeyCode _saveHotkey = KeyCode.F3;

        [SerializeField] private SaveMessage _saveMessage;

        public EditorState State => _state;
        public Action<EditorState> OnStateChange;
        public Action<PlayState> OnPlay;

        private void Start() {
            _save.onClick.AddListener(Save);
            _tilemap.onClick.AddListener(EnableTilemap);
            _obstacle.onClick.AddListener(EnableObstacle);
            _play.onClick.AddListener(Play);
            _continue.onClick.AddListener(Continue);
            _exit.onClick.AddListener(Exit);
            _return.onClick.AddListener(Return);
            _continue.interactable = false;
            _exit.interactable = false;
        }

        private void Update() {
            if (Input.GetKeyDown(_tilemapHotkey)) {
                EnableTilemap();
                return;
            }
            if (Input.GetKeyDown(_obstacleHotkey)) {
                EnableObstacle();
                return;
            }
            if (Input.GetKeyDown(_saveHotkey)) {
                Save();
                return;
            }
        }

        private void EnableTilemap() {
            if (_state == EditorState.Tilemap) {
                _tilemapEditor.Close();
                _state = EditorState.None;
            } else {
                _tilemapEditor.Open();
                _state = EditorState.Tilemap;
            }
            _obstacleEditor.Close();
            OnStateChange?.Invoke(_state);
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
            OnStateChange?.Invoke(_state);
        }

        private void Save() {
            SaveManager.Instance.Save();
            _saveMessage.Show();
        }

        private void HideAllEditors() {
            _tilemapEditor.Close();
            _obstacleEditor.Close();
            _state = EditorState.None;
            OnStateChange?.Invoke(_state);
        }

        private void Play() {
            OnPlay?.Invoke(PlayState.Begin);
            EventSystem.current.SetSelectedGameObject(null);
            _play.interactable = false;
            _continue.interactable = false;
            _exit.interactable = true;
            _editorButtons.FadeCanvas(2.0f, true, this);
            HideAllEditors();
        }

        private void Continue() {
            OnPlay?.Invoke(PlayState.Continue);
            EventSystem.current.SetSelectedGameObject(null);
            _exit.interactable = true;
            _play.interactable = false;
            _continue.interactable = false;
            _editorButtons.FadeCanvas(2.0f, true, this);
            HideAllEditors();
        }

        private void Exit() {
            OnPlay?.Invoke(PlayState.Exit);
            EventSystem.current.SetSelectedGameObject(null);
            _play.interactable = true;
            _continue.interactable = true;
            _exit.interactable = false;
            _editorButtons.FadeCanvas(2.0f, false, this);
        }

        private void Return() {
            print("return");
            SceneManager.LoadScene(MainMenuIndex);
        }
    }
}