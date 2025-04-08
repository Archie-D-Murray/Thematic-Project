using System.Collections;
using System.Collections.Generic;

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
        
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<TextMeshPro>().text = TitleText;
        enabled = true;
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
        enabled = true;
    }

    //TODO
    // menu pop up when player die/complete level
    // level end prefab
}
