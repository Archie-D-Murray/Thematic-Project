using System;

using UnityEngine;

namespace Data {
    [Serializable]
    public class SpawnPointData {
        public Vector3 Position;

        public SpawnPointData(Vector3 position) {
            Position = position;
        }
    }
}