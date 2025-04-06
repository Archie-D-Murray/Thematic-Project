using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Data;

using LevelEditor;

using TMPro;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.UI;

public class MenuLevelLoader : MonoBehaviour {
    [SerializeField] private Button saveButton;
    [SerializeField] private Button levelButtonPrefab;
    [SerializeField] private TMP_InputField levelNameInput;
    [SerializeField] private Transform selectionArea;
    [SerializeField] private string _saveDirectory = "Levels";
    [SerializeField] private List<Button> levelButtonList = new List<Button>();
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private ObstacleEditor _obstacleEditor;
    public float Alpha => canvas.alpha;
    // Start is called before the first frame update
    private void Awake() {
        canvas = GetComponent<CanvasGroup>();
        _obstacleEditor = FindFirstObjectByType<ObstacleEditor>();
        _obstacleEditor.UpdateSpawnPoint += CanSave;
        levelNameInput.characterValidation = TMP_InputField.CharacterValidation.Alphanumeric;
        saveButton.onClick.AddListener(() => SaveLevel());
        UpdateLevelUI();
        CanSave(_obstacleEditor.HasSpawnPoint);
    }

    private void SaveLevel() {
        List<string> fileList = new List<string>();
        foreach (string file in Directory.GetFiles(Path.Combine(Application.dataPath, _saveDirectory))) {
            if (File.Exists(file) && file.EndsWith("json")) {
                fileList.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        if (!fileList.Contains(levelNameInput.text) && _obstacleEditor.HasSpawnPoint) {
            SaveManager.Instance.Save(levelNameInput.text);
            UpdateLevelUI();
        } else if (!_obstacleEditor.HasSpawnPoint) {
            // TODO: Show error message
        }
    }

    private void CanSave(bool hasSpawnPoint) {
        saveButton.interactable = hasSpawnPoint;
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
        SaveManager.Instance.Load();
    }
}