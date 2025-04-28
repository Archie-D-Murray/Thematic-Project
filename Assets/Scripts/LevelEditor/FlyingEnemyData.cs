using System;

using UnityEngine;

namespace Data {
    [Serializable]
    public class StaticEnemyData {
        public Vector3 CurrentPosition;

        public StaticEnemyData(Vector3 currentPosition) {
            CurrentPosition = currentPosition;
        }
    }
}