using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data {
    [Serializable]
    public class SwingSD {
        public float rotateSpeed;
        public Vector3Int Position;

        public SwingSD(float RotationSpeed, Vector3Int position) {
            rotateSpeed = RotationSpeed;
            Position = position;
        }
    }
}