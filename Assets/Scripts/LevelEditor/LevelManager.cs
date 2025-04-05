using Data;

using UnityEngine;

namespace LevelEditor {
    public class LevelLoader : MonoBehaviour {
        private void Awake() {
            SaveManager.Instance.Load();
        }
    }
}