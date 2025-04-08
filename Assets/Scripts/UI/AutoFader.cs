using System.Collections;

using UnityEngine;

using UnityEngine.UI;

using Utilities;

namespace UI {

    public class AutoFader : MonoBehaviour {

        [SerializeField] private Image _fade;

        public void Awake() {
            _fade = GetComponent<Image>();
            _fade.color = Color.black;
            StartCoroutine(Fade());
        }

        private IEnumerator Fade() {
            float timer = 0f;
            while (timer <= 1f) {
                timer += Time.fixedDeltaTime;
                _fade.color = Color.Lerp(Color.black, Color.clear, timer);
                yield return Yielders.WaitForFixedUpdate;
            }
            _fade.color = Color.clear;
        }
    }
}