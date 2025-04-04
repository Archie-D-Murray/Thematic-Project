using System;

using UnityEngine;

namespace Data {
    [Serializable]
    public class LaserData {
        public Vector3 Position;

        public LaserData(Vector3 position) {
            Position = position;
        }
    }
}