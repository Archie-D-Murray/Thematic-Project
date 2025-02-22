using UnityEngine;

using Utilities;

using System.Linq;
using System.IO;

namespace Data {
    public class SaveManager : Singleton<SaveManager> {
        [SerializeField] private LevelData _data;
        [SerializeField] private string _file = "Level.json";

        [SerializeField] private string _path;

        private void Start() {
            _path = Path.Combine(Application.dataPath, _file);
        }

        private ISerialize[] GetSerializableTargets() {
            return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISerialize>().ToArray();
        }

        public void Save() {
            _data = new LevelData();
            foreach (ISerialize serializeTarget in GetSerializableTargets()) {
                serializeTarget.OnSave(ref _data);
            }
            if (File.Exists(_path)) {
                File.Delete(_path);
            }
            File.WriteAllText(_path, JsonUtility.ToJson(_data, true));
        }

        public void Load() {
            if (!File.Exists(_path)) {
                return;
            }
            _data = JsonUtility.FromJson<LevelData>(File.ReadAllText(_path));
            if (_data == null) {
                Debug.LogError($"Could not load save file: {_file}", this);
                return;
            }
            foreach (ISerialize serializeTarget in GetSerializableTargets()) {
                serializeTarget.OnLoad(_data);
            }
        }
    }
}