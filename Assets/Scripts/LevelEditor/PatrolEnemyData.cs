using System;

using UnityEngine;

namespace Data {
    [Serializable]
    public class PatrolEnemyData {
        public Vector3 CurrentPosition;
        public Vector3 Patrol1;
        public Vector3 Patrol2;

        public PatrolEnemyData(Vector3 currentPosition, Vector3 patrol1, Vector3 patrol2) {
            CurrentPosition = currentPosition;
            Patrol1 = patrol1;
            Patrol2 = patrol2;
        }
    }
}