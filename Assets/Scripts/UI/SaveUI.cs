using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Data;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class SaveUI : MonoBehaviour
{
    [SerializeField] private Button saveButton;
    [SerializeField] private Button levelButtonPrefab;
    [SerializeField] private TMP_InputField levelNameInput;
    [SerializeField] private Transform selectionArea;
    [SerializeField] private string _saveDirectory = "Levels";
    [SerializeField] private List<Button> levelButtonList = new List<Button>();
    // Start is called before the first frame update
    private void Start()
    {
        ShowLevels();
        levelNameInput.characterValidation = TMP_InputField.CharacterValidation.Alphanumeric;
        saveButton.onClick.AddListener(() => SaveLevel());
    }

    private void SaveLevel() {
        List<string> fileList = new List<string>();
        foreach (string file in Directory.GetFiles(Path.Combine(Application.dataPath, _saveDirectory))) {
            if (File.Exists(file) && file.EndsWith("json")) {
                string[] fileLine = file.Split('\\');
                fileList.Add(fileLine[fileLine.Length - 1].Replace(".json", ""));
            }
        }

        if (!fileList.Contains(levelNameInput.text)) {
            SaveManager.Instance.Save(levelNameInput.text);
            ShowLevels();
        }
    }

    private void ShowLevels() {
        if(levelButtonList.Count > 0) {
            foreach(Button button in  levelButtonList) {
                Destroy(button);
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

    private void LoadLevel(string levelName) {
        Debug.Log($"Loading level: {levelName}");
        SaveManager.Instance.Load(levelName);
    }
}
