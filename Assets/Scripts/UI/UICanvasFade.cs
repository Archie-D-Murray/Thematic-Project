using UnityEngine;

using Utilities;

namespace UI {
    [RequireComponent(typeof(CanvasGroup))]
    public class UICanvasFade : MonoBehaviour {
        [SerializeField] private float _fadeSpeed = 1.0f;
        [SerializeField] private bool _hideOnAwake = true;
        private CanvasGroup _canvas;

        private void Awake() {
            _canvas = GetComponent<CanvasGroup>();
            if (_hideOnAwake) {
                Hide();
            }
        }

        public void Show() {
            _canvas.FadeCanvas(_fadeSpeed, false, this);
        }

        public void Hide() {
            _canvas.FadeCanvas(_fadeSpeed, true, this);
        }
    }
}