using UnityEngine;
using UnityEngine.EventSystems;

using Utilities;

namespace UI {
    public class UIManager : Singleton<UIManager> {
        public bool IsHovered() {
            return EventSystem.current.IsPointerOverGameObject(); // GameObject in question is canvas...
        }
    }
}