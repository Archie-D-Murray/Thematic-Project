using System;

using UnityEngine;

namespace LevelEditor {
    public abstract class Placeable : MonoBehaviour {
        [SerializeField] protected Transform[] _moveables;
        [SerializeField] protected bool _playing = false;
        [SerializeField] protected Vector3 _initialPosition;

        protected Action OnPlaceStart = delegate { };
        protected Action OnPlaceFinish = delegate { };

        protected abstract Transform[] GetMoveables();

        public int MoveableCount => _moveables.Length;

        public void InitReferences() {
            if (_moveables == null || _moveables.Length < 1) {
                _moveables = GetMoveables();
            }
            _initialPosition = GetInitial().position;
        }

        public void StartPlacement() {
            OnPlaceStart?.Invoke();
            _initialPosition = GetInitial().position;
        }

        public void FinishPlacement() {
            OnPlaceFinish?.Invoke();
            _initialPosition = GetInitial().position;
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

        public virtual void RemovePlaceable() {
            Destroy(gameObject);
        }

        public virtual void EnterPlayMode() {
            _playing = true;
            GetInitial().position = _initialPosition;
        }

        public virtual void ExitPlayMode() {
            _playing = false;
        }

        public virtual void ContinuePlayMode() {
            _playing = true;
        }

        public void OnPlay(PlayState state) {
            switch (state) {
                case PlayState.Begin:
                    EnterPlayMode();
                    break;
                case PlayState.Continue:
                    ContinuePlayMode();
                    break;
                case PlayState.Exit:
                    ExitPlayMode();
                    break;
                default:
                    Debug.LogWarning($"Unknown play state encountered: {state}", this);
                    break;
            }
        }
    }
}