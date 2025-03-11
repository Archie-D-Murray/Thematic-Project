using System;

using UnityEngine;

namespace Data {
    [Serializable]
    public class DoorData {
        public Vector3 DoorPosition;
        public Vector3 ButtonPosition;
        public bool Open;

        public DoorData(Vector3 doorPosition, Vector3 buttonPosition, bool open) {
            DoorPosition = doorPosition;
            ButtonPosition = buttonPosition;
            Open = open;
        }
    }
}