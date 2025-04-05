using UnityEngine;

namespace Utilities {

    public class PersistentSingleton<T> : MonoBehaviour where T : Component {
        [Tooltip("If this is true, this singleton will auto detach if it finds itself parented on awake")]
        public bool UnparentOnAwake = true;

        public static bool HasInstance => _internalInstance != null;
        public static T Current => _internalInstance;

        protected static T _internalInstance;

        public static T Instance {
            get {
                if (_internalInstance == null) {
                    _internalInstance = FindFirstObjectByType<T>();
                    if (_internalInstance == null) {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name + "AutoCreated";
                        _internalInstance = obj.AddComponent<T>();
                    }
                }

                return _internalInstance;
            }
        }

        public static void StartSingleton() {
            if (_internalInstance == null) {
                _internalInstance = FindFirstObjectByType<T>();
                if (_internalInstance == null) {
                    GameObject obj = new GameObject();
                    obj.name = $"{typeof(T).Name} - AutoCreated";
                    _internalInstance = obj.AddComponent<T>();
                    DontDestroyOnLoad(Instance.gameObject);
                }
            }
        }

        protected virtual void Awake() => InitializeSingleton();

        protected virtual void InitializeSingleton() {
            if (!Application.isPlaying) {
                return;
            }

            if (UnparentOnAwake) {
                transform.SetParent(null);
            }

            if (_internalInstance == null) {
                _internalInstance = this as T;
                DontDestroyOnLoad(transform.gameObject);
                enabled = true;
            } else {
                if (this != _internalInstance) {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}