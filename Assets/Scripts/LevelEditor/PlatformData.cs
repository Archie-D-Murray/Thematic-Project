using System;

using UnityEngine;

namespace Data {
    [Serializable]
    public class PlatformData {
        public Vector3 CurrentPosition;
        public Vector3 Pos1;
        public Vector3 Pos2;

        public PlatformData(Vector3 currentPosition, Vector3 pos1, Vector3 pos2) {
            CurrentPosition = currentPosition;
            Pos1 = pos1;
            Pos2 = pos2;
        }
    }
}