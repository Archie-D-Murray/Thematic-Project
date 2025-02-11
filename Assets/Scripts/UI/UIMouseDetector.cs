using UnityEngine;
using UnityEngine.EventSystems;

namespace UI {
    [RequireComponent(typeof(CanvasGroup))]
    public class UIMouseDetector : MonoBehaviour {
        [SerializeField] private CanvasGroup canvas;

        private void Start() {
            canvas = GetComponent<CanvasGroup>();
        }

        public bool IsHovered() {
            return EventSystem.current.IsPointerOverGameObject() && canvas.alpha == 1f;
        }
    }
}