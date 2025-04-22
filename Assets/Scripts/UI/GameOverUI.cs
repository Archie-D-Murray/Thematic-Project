using System.Collections;
using System.Collections.Generic;

using Entity.Player;

using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] Button RestartButton;
    [SerializeField] Button MenuButton;
    [SerializeField] int MainMenuIndex;
    [SerializeField] string TitleText;
    [SerializeField] private CanvasGroup _canvas;
    private PlayerController _playerController;
        
    // Start is called before the first frame update
    void Start()
    {
        _canvas = GetComponent<CanvasGroup>();
        _canvas.FadeCanvas(100.0f, true, this);
        GetComponentInChildren<TMP_Text>().text = TitleText;
        RestartButton.onClick.AddListener(() => Restart());
        MenuButton.onClick.AddListener(() => Menu());
        _playerController = FindFirstObjectByType<PlayerController>();
        _playerController.OnDeath += PopUp;
    }

    public void Menu() {
        //Load Main Menu scene
        SceneManager.LoadScene(MainMenuIndex);
    }

    public void Restart() {
        //Reload level data here
        print("restart");
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void PopUp() {
        Debug.Log("Player died");
        _canvas.FadeCanvas(2.0f, false, this);
    }

}
