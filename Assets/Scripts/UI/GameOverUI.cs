using System.Collections;
using System.Collections.Generic;

using Entity.Player;

using TMPro;
using UI;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

public class GameOverUI : MonoBehaviour
{
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
        RestartButton.onClick.AddListener(() => Restart());
        MenuButton.onClick.AddListener(() => Menu());
        _playerController = FindFirstObjectByType<PlayerController>();
        _playerController.OnDeath += PopUp;
        _currentScene = SceneManager.GetActiveScene().buildIndex;
    }

    public void setText(string text) {
        Title.text = text;
    }

    public void Menu() {
        //Load Main Menu scene
        SceneManager.LoadScene(LevelIndex.Menu);
    }

    public void Restart() {
        SceneManager.LoadScene(_currentScene);
    }

    public void PopUp() {
        StartCoroutine(WaitForDeathAnimation(_playerController.DeathTime));
    }

    private IEnumerator WaitForDeathAnimation(float duration) {
        yield return Yielders.WaitForSeconds(duration);
        _canvas.FadeCanvas(2.0f, false, this);
    }

    public void Close() {
        _canvas.FadeCanvas(100.0f, true, this);
    }

}
