using System.Collections;
using System.Collections.Generic;

using Entity.Player;

using TMPro;

using UI;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Utilities;

public class GameOverUI : MonoBehaviour {
    [SerializeField] Button RestartButton;
    [SerializeField] Button MenuButton;
    [SerializeField] int _currentScene;
    [SerializeField] string TitleText;
    [SerializeField] private CanvasGroup _canvas;
    private PlayerController _playerController;

    // Start is called before the first frame update
    void Start() {
        _canvas = GetComponent<CanvasGroup>();
        _canvas.FadeCanvas(100.0f, true, this);
        GetComponentInChildren<TMP_Text>().text = TitleText;
        RestartButton.onClick.AddListener(() => Restart());
        MenuButton.onClick.AddListener(() => Menu());
        _playerController = FindFirstObjectByType<PlayerController>();
        _playerController.OnDeath += PopUp;
        _currentScene = SceneManager.GetActiveScene().buildIndex;
    }

    public void Menu() {
        //Load Main Menu scene
        SceneManager.LoadScene(LevelIndex.Menu);
    }

    public void Restart() {
        //Reload level data here

        SceneManager.LoadScene(_currentScene);
    }

    public void PopUp() {
        StartCoroutine(WaitForDeathAnimation(_playerController.DeathTime));
    }

    private IEnumerator WaitForDeathAnimation(float duration) {
        yield return Yielders.WaitForSeconds(duration);
        _canvas.FadeCanvas(2.0f, false, this);
    }

    //TODO
    // menu pop up when player die/complete level
    // level end prefab
}