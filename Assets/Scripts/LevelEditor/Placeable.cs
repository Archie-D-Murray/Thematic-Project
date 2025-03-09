using UnityEngine;

namespace LevelEditor {
    public abstract class Placeable : MonoBehaviour {
        [SerializeField] protected Transform[] _moveables;
        [SerializeField] protected bool _placing = false;

        protected abstract Transform[] GetMoveables();

        public void InitReferences() {
            if (_moveables == null || _moveables.Length < 1) {
                Debug.Log("Manually inited Moveables");
                _moveables = GetMoveables();
            }
        }

        public void StartPlacement() {
            _placing = true;
        }

        public void FinishPlacement() {
            _placing = false;
        }

        public Transform GetInitial() {
            return _moveables[0];
        }

        public Transform GetNext(ref int index) {
            index = ++index % _moveables.Length;
            return _moveables[index];
        }
        public Transform GetPrev(ref int index) {
            if (index <= 0) {
                index = _moveables.Length - 1;
            } else {
                index--;
            }
            return _moveables[index];
        }
    }
}