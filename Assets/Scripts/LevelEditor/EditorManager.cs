using UnityEngine;
using UnityEngine.UI;

using System.Linq;
using System;
using Utilities;
using UnityEngine.EventSystems;
using Data;
using UI;
using UnityEngine.SceneManagement;
using Game;
using System.Collections;

namespace LevelEditor {

    public enum EditorState { None, Obstacle, Tilemap, Tutorial }

    public enum PlayState { Begin, Continue, Exit }

    public class EditorManager : Singleton<EditorManager> {

        [SerializeField] private Button _other;
        [SerializeField] private Button _tilemap;
        [SerializeField] private Button _obstacle;
        [SerializeField] private Button _play;
        [SerializeField] private Button _continue;
        [SerializeField] private Button _exit;
        [SerializeField] private Button _return;

        [SerializeField] private CanvasGroup _editorButtons;

        [SerializeField] private ObstacleEditor _obstacleEditor;
        [SerializeField] private TilemapEditor _tilemapEditor;
        [SerializeField] private TutorialManager _tutorialManager;
        [SerializeField] private EditorState _state;

        [SerializeField] private KeyCode _tilemapHotkey = KeyCode.F1;
        [SerializeField] private KeyCode _obstacleHotkey = KeyCode.F2;
        [SerializeField] private KeyCode _saveHotkey = KeyCode.F3;

        [SerializeField] private SaveMessage _saveMessage;
        [SerializeField] private bool _isTutorial;

        public EditorState State => _state;
        public Action<EditorState> OnStateChange;
        public Action<PlayState> OnPlay;

        private void Start() {
            _other.onClick.AddListener(Other);
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
                Other();
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
            if (_isTutorial) {
                _tutorialManager.Close();
            }
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
            _tilemapEditor.Close();
            if (_isTutorial) {
                _tutorialManager.Close();
            }
            OnStateChange?.Invoke(_state);
        }

        private void Other() {
            if (_isTutorial) {
                if (_state == EditorState.Tutorial) {
                    _tutorialManager.Close();
                    _state = EditorState.None;
                } else {
                    _tutorialManager.Open();
                    _state = EditorState.Tutorial;
                }
            } else {
                SaveManager.Instance.Save();
                _saveMessage.Show();
            }
        }

        private void HideAllEditors() {
            _tilemapEditor.Close();
            _obstacleEditor.Close();
            if (_isTutorial) {
                _tutorialManager.Close();
            }
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
            FadeScreen.Instance.Black(LevelIndex.Menu);
        }

        private IEnumerator FadeScene(int sceneIndex, float fadeTime) {
            yield return Yielders.WaitForSeconds(fadeTime);
            SceneManager.LoadScene(sceneIndex);
        }
    }
}