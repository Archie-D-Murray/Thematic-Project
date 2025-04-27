using System.Collections;
using System.Collections.Generic;

using Entity.Player;

using LevelEditor;

using TMPro;

using UI;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Utilities;

public class GameOverUI : MonoBehaviour {
    [SerializeField] private KeyCode _menuKey = KeyCode.Tab;
    [SerializeField] Button RestartButton;
    [SerializeField] Button MenuButton;
    [SerializeField] int _currentScene;
    [SerializeField] public TMP_Text Title;
    [SerializeField] private CanvasGroup _canvas;
    private PlayerController _playerController;

    // Start is called before the first frame update
    void Start() {
        _canvas = GetComponent<CanvasGroup>();
        _canvas.FadeCanvas(100.0f, true, this);
        Title = GetComponentInChildren<TMP_Text>();
        RestartButton.onClick.AddListener(() => Restart());
        MenuButton.onClick.AddListener(() => Menu());
        _playerController = FindFirstObjectByType<PlayerController>();
        _playerController.OnDeath += Death;
        _currentScene = SceneManager.GetActiveScene().buildIndex;
        if (_currentScene != LevelIndex.Game) {
            EditorManager.Instance.OnPlay += CloseOnEditorMode;
        } else {
            GetComponentInChildren<Tags.UI.ReadoutTag>().GetComponent<Button>().onClick.AddListener(Close);
        }
    }

    private void CloseOnEditorMode(PlayState mode) {
        if (mode == PlayState.Exit) {
            Close();
        }
    }

    public void Menu() {
        FadeScreen.Instance.Black(LevelIndex.Menu);
    }

    public void Restart() {
        FadeScreen.Instance.Black(_currentScene);
    }

    public void Death() {
        Title.text = "You Died!";
        StartCoroutine(WaitForDeathAnimation(_playerController.DeathTime));
    }

    public void Win() {
        Title.text = "You Win!";
        _canvas.FadeCanvas(Extensions.FadeSpeed, false, this);
    }

    private IEnumerator WaitForDeathAnimation(float duration) {
        yield return Yielders.WaitForSeconds(duration);
        _canvas.FadeCanvas(Extensions.FadeSpeed, false, this);
    }

    public void Close() {
        _canvas.FadeCanvas(100.0f, true, this);
    }

    private void Update() {
        if (_currentScene == LevelIndex.Game && Input.GetKeyDown(_menuKey) && _canvas.alpha == 0.0f) {
            Title.text = string.Empty;
            _canvas.FadeCanvas(Extensions.FadeSpeed, false, this);
        }
    }
}