using Data;

using Entity.Player;

using UnityEngine;

namespace LevelEditor {
    public class SpawnPoint : Placeable {

        private void Awake() {
            FindObjectOfType<PlayerController>().transform.position = transform.position;
        }

        protected override Transform[] GetMoveables() {
            return new Transform[] { transform };
        }

        public void LoadSaveData(SpawnPointData data) {
            transform.position = data.Position;
        }

        public SpawnPointData ToSaveData() {
            return new SpawnPointData(transform.position);
        }
    }
}