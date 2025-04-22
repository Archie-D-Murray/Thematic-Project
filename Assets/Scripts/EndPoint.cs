using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entity.Player;
using LevelEditor;
using Data;
namespace LevelEditor {
    public class EndPoint : Placeable {

        public GameOverUI Menu;

        public void Start() {
            Menu = FindAnyObjectByType<GameOverUI>();
        }
        protected override Transform[] GetMoveables() {
            return new Transform[] { transform };
        }

        public void LoadSaveData(SpawnPointData data) {
            transform.position = data.Position;
        }

        public SpawnPointData ToSaveData() {
            return new SpawnPointData(_initialPosition);
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.HasComponent<PlayerController>()) {
                Menu.PopUp();
            }
        }

        //TO DO
        //change screen text
        //placeable in editor
    }
}
