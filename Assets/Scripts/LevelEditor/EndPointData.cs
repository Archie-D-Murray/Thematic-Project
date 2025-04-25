using System;

using UnityEngine;

namespace Data {
    [Serializable]
    public class EndPointData {
        public Vector3 Position;

        public EndPointData(Vector3 position) {
            Position = position;
        }
    }
}