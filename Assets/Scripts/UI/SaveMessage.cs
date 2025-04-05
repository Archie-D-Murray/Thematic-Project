using UnityEngine;

using TMPro;

using Data;

using Utilities;
using System;

namespace UI {
    public class SaveMessage : MonoBehaviour {

        [SerializeField] private TMP_Text _message;
        [SerializeField] private CanvasGroup _canvas;
        [SerializeField] private float _fadeSpeed = 0.5f;

        private void Start() {
            _canvas.alpha = 0.0f;
        }

        public void Show() {
            _canvas.alpha = 1.0f;
            _message.text = $"Saved level: {SaveManager.Instance.LevelName}";
            _canvas.FadeCanvas(_fadeSpeed, true, this);
        }
    }
}