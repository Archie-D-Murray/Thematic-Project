using UnityEngine;

using Utilities;

using System.Linq;
using System.IO;
using UnityEngine.UI;

namespace Data {
    public class SaveManager : Singleton<SaveManager> {
        [SerializeField] private LevelData _data;
        [SerializeField] private string _saveDirectory = "Levels";

        private void Start() {
            if (!Directory.Exists(Path.Combine(Application.dataPath, _saveDirectory))) {
                Directory.CreateDirectory(Path.Combine(Application.dataPath, _saveDirectory));
            }
            // foreach (string file in Directory.GetFiles(Path.Combine(Application.dataPath, _saveDirectory))) {
            //     if (File.Exists(file) && file.EndsWith("json")) {
            //         Debug.Log($"Found file {file}");
            //     }
            // }
        }

        private ISerialize[] GetSerializableTargets() {
            return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISerialize>().ToArray();
        }

        public void Save(string levelName) {
            string path = GetFilePath(levelName);
            _data = new LevelData();
            foreach (ISerialize serializeTarget in GetSerializableTargets()) {
                serializeTarget.OnSave(ref _data);
            }
            if (File.Exists(path)) {
                File.Delete(path);
            }
            File.WriteAllText(path, JsonUtility.ToJson(_data, true));
        }

        public void Load(string levelName) {
            string path = GetFilePath(levelName);
            if (!File.Exists(path)) {
                return;
            }
            _data = JsonUtility.FromJson<LevelData>(File.ReadAllText(path));
            if (_data == null) {
                Debug.LogError($"Could not load save file: {path}", this);
                return;
            }
            foreach (ISerialize serializeTarget in GetSerializableTargets()) {
                serializeTarget.OnLoad(_data);
            }
        }

        private string GetFilePath(string levelName) {
            return Path.Combine(Application.dataPath, _saveDirectory, $"{levelName}.json");
        }
    }
}