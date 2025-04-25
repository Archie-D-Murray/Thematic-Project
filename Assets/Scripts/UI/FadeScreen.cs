using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Utilities;

namespace UI {

    public class FadeScreen : Singleton<FadeScreen> {

        [SerializeField] private Image _fade;

        protected override void Awake() {
            base.Awake();
            _fade = GetComponent<Image>();
            _fade.color = Color.black;
            StartCoroutine(Fade());
        }

        private IEnumerator Fade(bool fadeToTransparent = true) {
            float timer = 0f;
            Color initial = fadeToTransparent ? Color.black : Color.clear;
            Color target = fadeToTransparent ? Color.clear : Color.black;
            while (timer <= 1f) {
                timer += Time.fixedDeltaTime;
                _fade.color = Color.Lerp(initial, target, timer);
                yield return Yielders.WaitForFixedUpdate;
            }
            _fade.color = target;
        }

        private IEnumerator FadeSceneChange(int sceneIndex, bool fadeToTransparent = true) {
            float timer = 0f;
            Color initial = fadeToTransparent ? Color.black : Color.clear;
            Color target = fadeToTransparent ? Color.clear : Color.black;
            while (timer <= 1f) {
                timer += Time.fixedDeltaTime;
                _fade.color = Color.Lerp(initial, target, timer);
                yield return Yielders.WaitForFixedUpdate;
            }
            _fade.color = target;
            SceneManager.LoadScene(sceneIndex);
        }

        public void Black(int sceneIndex) {
            StartCoroutine(FadeSceneChange(sceneIndex, false));
        }

        public void Clear() {
            StartCoroutine(Fade(true));
        }
    }
}