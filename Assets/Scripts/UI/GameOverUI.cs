using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] Button RestartButton;
    [SerializeField] Button MenuButton;
        
    // Start is called before the first frame update
    void Start()
    {
        RestartButton.onClick.AddListener(() => Restart());
        MenuButton.onClick.AddListener(() => Menu());
    }

    private void Menu() {
        //Load Main Menu scene
        print("Back to Menu");
    }

    private void Restart() {
        //Reload level data here
        print("restart");
    }

    //TODO
    // menu pop up when player die/complete level
    // level end prefab
}
