using Entity.Player;

using UnityEngine;

namespace Game {
    public class PlayerLoader : MonoBehaviour {
        private void Start() {
            FindFirstObjectByType<PlayerController>().OnPlay(LevelEditor.PlayState.Begin);
        }
    }
}