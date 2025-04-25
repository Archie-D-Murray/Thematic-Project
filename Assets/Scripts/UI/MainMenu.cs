using UnityEngine;
using UnityEngine.UI;

using Data;
using UnityEngine.SceneManagement;
using Utilities;
using System.Collections;
using UnityEngine.Audio;
using TMPro;

namespace UI {

    public enum MenuMode { Game, Edit }

    public struct LevelIndex {
        public const int Menu = 0;
        public const int Edit = 1;
        public const int Game = 2;
        public const int Tutorial = 3;
    }

    public class MainMenu : MonoBehaviour {

        private const string MASTER_VOLUME = "MasterVolume";
        private const string BGM_VOLUME = "BGMVolume";
        private const string SFX_VOLUME = "SFXVolume";

        [SerializeField] private TMP_InputField _saveName;
        [SerializeField] private MenuLevelLoader _levelLoader;
        [SerializeField] private MenuMode _mode;
        [SerializeField] private Image _fade;
        [SerializeField] private float _transitionTime = 1.0f;
        [SerializeField] private AudioMixer _mixer;

        private void Start() {
            _levelLoader.Init(this);
        }

        public void GameMode() {
            _mode = MenuMode.Game;
        }

        public void EditorMode() {
            _mode = MenuMode.Edit;
        }

        public void CreateSave() {
            SaveManager.Instance.LevelName = _saveName.text;
            SaveManager.Instance.InitialSave = true;
            FadeScreen.Instance.Black(LevelIndex.Edit);
        }

        public void LoadSave() {
            if (_mode == MenuMode.Game) {
                FadeScreen.Instance.Black(LevelIndex.Game);
            } else {
                FadeScreen.Instance.Black(LevelIndex.Edit);
            }
        }

        private IEnumerator LoadSceneDelayed(float delay, int sceneIndex) {
            float time = 0.0f;
            while (time < delay) {
                Debug.Log($"Fading: {(time / delay):0%}");
                _fade.color = Color.Lerp(Color.clear, Color.black, time / delay);
                time += Time.fixedDeltaTime;
                yield return Yielders.WaitForFixedUpdate;
            }
            Debug.Log("Loading scene...");
            _fade.color = Color.black;
            SceneManager.LoadScene(sceneIndex);
        }

        public void MasterVolume(float value) {
            _mixer.SetFloat(MASTER_VOLUME, 20 * Mathf.Log10(value));
        }
        public void BGMVolume(float value) {
            _mixer.SetFloat(BGM_VOLUME, 20 * Mathf.Log10(value));
        }
        public void SFXVolume(float value) {
            _mixer.SetFloat(SFX_VOLUME, 20 * Mathf.Log10(value));
        }

        public void Tutorial() {
            FadeScreen.Instance.Black(LevelIndex.Tutorial);
        }

        public void Quit() {
            StartCoroutine(QuitFade());
        }

        private IEnumerator QuitFade() {
            float timer = 0f;
            while (timer <= 1) {
                timer += Time.fixedDeltaTime;
                _fade.color = Color.Lerp(Color.clear, Color.black, timer);
                yield return Yielders.WaitForFixedUpdate;
            }
            _fade.color = Color.black;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }
    }
}