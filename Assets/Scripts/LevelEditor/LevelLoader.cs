using Data;

using UnityEngine;

namespace LevelEditor {
    public class LevelManager : MonoBehaviour {
        private void Awake() {
            SaveManager.Instance.Load();
        }
    }
}