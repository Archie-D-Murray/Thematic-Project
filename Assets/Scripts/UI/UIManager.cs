using UnityEngine;

using Utilities;

namespace UI {
    public class UIManager : Singleton<UIManager> {
        [SerializeField] private UIMouseDetector[] _uiDetectors;

        private void Start() {
            _uiDetectors = FindObjectsOfType<UIMouseDetector>();
        }

        public bool IsHovered() {
            foreach (UIMouseDetector detector in _uiDetectors) {
                if (detector.IsHovered()) {
                    return true;
                }
            }
            return false;
        }
    }
}