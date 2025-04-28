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

public class LevelTimer : MonoBehaviour {
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text bestTimeText;

    [SerializeField] private TimerData _data;

    private PlayerController player;
    protected float timer;
    protected string path;
    private float bestTime;
    private bool _running = true;

    Dictionary<string, float> _dict = new Dictionary<string, float>();

    private void Start() {
        player = FindFirstObjectByType<PlayerController>();
        player.OnWin += End;
        path = Path.Combine(Application.dataPath, "Times.json");

        Load();
        _running = true;

        if (_dict.ContainsKey(SaveManager.Instance.LevelName)) {
            bestTime = _dict[SaveManager.Instance.LevelName];
            bestTimeText.text = ("Best Time: " + (Mathf.Round(bestTime * 100) / 100.0).ToString());
        } else {
            bestTime = Mathf.Infinity;
            bestTimeText.text = "Best Time: No data";
        }
    }

    private void Update() {
        if (!_running) {
            return;
        }
        timer += Time.deltaTime;
        timerText.text = (Mathf.Round(timer * 100) / 100.0).ToString();
    }

    public void End() {
        _running = false;
        if (timer <= bestTime) {
            bestTimeText.text = ("Best Time: " + (Mathf.Round(timer * 100) / 100.0).ToString());
            _dict[SaveManager.Instance.LevelName] = timer;
            print(_dict[SaveManager.Instance.LevelName]);
            Save();
        }
    }

    public void Save() {
        _data = new TimerData(_dict.Keys.ToList(), _dict.Values.ToList());

        if (File.Exists(path)) {
            File.Delete(path);
        }

        File.WriteAllText(path, JsonUtility.ToJson(_data, true));

        print("time saved");
    }

    public void Load() {
        if (File.Exists(path)) {
            _data = JsonUtility.FromJson<TimerData>(File.ReadAllText(path));
        } else {
            _data = new TimerData();
        }
        _dict.Clear();
        for (int i = 0; i < _data.names.Count; i++) {
            _dict.Add(_data.names[i], _data.times[i]);
        }
        print(_dict);
    }

    // TODO  - loading times
}

[System.Serializable]
public class TimerData {
    public List<string> names;
    public List<float> times;

    public TimerData() {
        names = new List<string>();
        times = new List<float>();
    }

    public TimerData(List<string> Names, List<float> Times) {
        names = Names;
        times = Times;
    }
}