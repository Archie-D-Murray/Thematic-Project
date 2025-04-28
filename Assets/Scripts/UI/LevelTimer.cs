using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

using Entity.Player;
using Data;
using System.Linq;
using System.IO;
using Utilities;
using System.Reflection;

public class LevelTimer : MonoBehaviour 
{
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text bestTimeText;

    [SerializeField] private timerData _data;

    private PlayerController player;
    protected float timer;
    protected string path;
    private float bestTime;

    Dictionary<string, float> _dict = new Dictionary<string, float>();

    private void Start() {
        player = FindFirstObjectByType<PlayerController>();
        player.OnWin += End;


        if (!Directory.Exists(Path.Combine(Application.dataPath, "Times"))) {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "Times"));
            print("created");
        } else {
            print("Times exists");
        }

        path = Path.Combine(Application.dataPath, "Times", $"times.json");

        Load();

        if (_dict.ContainsKey(SaveManager.Instance.LevelName)) { 
            bestTime = _dict[SaveManager.Instance.LevelName];
            bestTimeText.text = ("Best Time: " + (Mathf.Round(bestTime * 100) / 100.0).ToString());
        }
    }

    private void Update() {
        timer += Time.deltaTime;
        timerText.text = (Mathf.Round(timer * 100) / 100.0).ToString();
    }

    public void End() {
        if (timer < bestTime) {
            _dict[SaveManager.Instance.LevelName] = timer;
            print(_dict[SaveManager.Instance.LevelName]);
            Save();
        }
    }

    public void Save() {
        _data = new timerData(_dict.Keys.ToList(), _dict.Values.ToList());

        if (File.Exists(path)) {
            File.Delete(path);
        }

        File.WriteAllText(path, JsonUtility.ToJson(_data, true));

        print("time saved");
    }

    public void Load() {
        _data = JsonUtility.FromJson<timerData>(File.ReadAllText(path));

        for (int i = 0; i < _data.names.Count; i++) {
            _dict.Add(_data.names[i], _data.times[i]);
        }
        print(_dict);
    }

    // TODO  - loading times
}

[System.Serializable]
public class timerData {
    public List<string> names;
    public List<float> times;

    public timerData(List<string> Names, List<float> Times) {
        names = Names;
        times = Times;
    }
}