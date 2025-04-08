using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Data;

using LevelEditor;

using TMPro;

using UI;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.UI;

public class MenuLevelLoader : MonoBehaviour {
    [SerializeField] private Button levelButtonPrefab;
    [SerializeField] private Transform selectionArea;
    [SerializeField] private string _saveDirectory = "Levels";
    [SerializeField] private List<Button> levelButtonList = new List<Button>();
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private MainMenu _mainMenu;

    public float Alpha => canvas.alpha;
    // Start is called before the first frame update
    private void Awake() {
        canvas = GetComponent<CanvasGroup>();
        Close();
        UpdateLevelUI();
    }

    private void UpdateLevelUI() {
        if (levelButtonList.Count > 0) {
            foreach (Button button in levelButtonList) {
                Destroy(button.gameObject);
            }
            levelButtonList.Clear();
        }
        foreach (string file in Directory.GetFiles(Path.Combine(Application.dataPath, _saveDirectory))) {
            if (File.Exists(file) && file.EndsWith("json")) {
                string fileLine = Path.GetFileNameWithoutExtension(file);
                Button newLevel = Instantiate(levelButtonPrefab, selectionArea);
                TMP_Text levelText = newLevel.GetComponentInChildren<TMP_Text>();
                levelText.text = fileLine;
                newLevel.onClick.AddListener(() => LoadLevel(fileLine));
                levelButtonList.Add(newLevel);
            }
        }
    }

    public void Open() {
        if (canvas.alpha == 0) {
            canvas.FadeCanvas(2.0f, false, this);
            UpdateLevelUI();
            return;
        }
    }

    public void Close() {
        if (canvas.alpha == 1) {
            canvas.FadeCanvas(2.0f, true, this);
            return;
        }
    }

    private void LoadLevel(string levelName) {
        Debug.Log($"Loading level: {levelName}");
        SaveManager.Instance.LevelName = levelName;
        _mainMenu.LoadSave();
        Debug.Log("Loaded scene...");
    }

    public void Init(MainMenu mainMenu) {
        _mainMenu = mainMenu;
    }
}