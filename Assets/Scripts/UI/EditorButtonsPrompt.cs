using LevelEditor;

using UnityEngine;

using Utilities;

namespace UI {
    [RequireComponent(typeof(CanvasGroup))]
    public class EditorButtonsPrompt : MonoBehaviour {

        private CanvasGroup _canvas;
        private CountDownTimer _timer = new CountDownTimer(0.0f);

        private void Start() {
            _canvas = GetComponent<CanvasGroup>();
            _timer.OnTimerStop += () => Destroy(gameObject);
            EditorManager.Instance.OnStateChange += (EditorState _) => Hide();
        }

        public void Hide() {
            _timer.Reset(1.0f / Extensions.FadeSpeed);
            _canvas.FadeCanvas(Extensions.FadeSpeed, true, this);
        }
    }
}