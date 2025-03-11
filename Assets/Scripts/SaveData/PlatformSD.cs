using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// for Moving Platform and Projectiles

namespace Data {
    [Serializable]
    public class PlatformSD {
        public float Speed;
        public bool Reset;
        public Vector3Int Position;

        public PlatformSD(int speed, bool reset, Vector3Int position) {
            Speed = speed;
            Reset = reset;
            Position = position;
        }
    }
}
