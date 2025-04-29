using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entity.Player;
using LevelEditor;
using Data;
namespace LevelEditor {
    public class EndPoint : Placeable {

        private void Start() {
            InitReferences();
        }

        protected override Transform[] GetMoveables() {
            return new Transform[] { transform };
        }

        public void LoadSaveData(EndPointData data) {
            _initialPosition = data.Position;
            transform.position = _initialPosition;
        }

        public EndPointData ToSaveData() {
            return new EndPointData(_initialPosition);
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (_playing && collision.TryGetComponent(out PlayerController controller)) {
                controller.Win();
            }
        }
    }
}