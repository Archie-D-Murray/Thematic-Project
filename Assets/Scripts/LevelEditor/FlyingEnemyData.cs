using System;

using UnityEngine;

namespace Data {
    [Serializable]
    public class FlyingEnemyData {
        public Vector3 CurrentPosition;

        public FlyingEnemyData(Vector3 currentPosition) {
            CurrentPosition = currentPosition;
        }
    }
}