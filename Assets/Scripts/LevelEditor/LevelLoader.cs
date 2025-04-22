using System.Collections;

using Data;

using UnityEngine;

using Utilities;

namespace LevelEditor {
    public class LevelManager : MonoBehaviour {
        private void Awake() {
            Debug.Log($"Level loader: {name}");
            SaveManager.Instance.Load();
            if (SaveManager.Instance.InitialSave) {
                SaveManager.Instance.InitialSave = false;
                StartCoroutine(InitialSave());
            }
        }

        private IEnumerator InitialSave() {
            yield return Yielders.WaitForEndOfFrame;
            SaveManager.Instance.Save();
        }
    }
}