using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data {
    [Serializable]
    public class RespawnSD {
        public Vector3int Position;

        public RespawnSD(Vector3int position) {
            Position = position;
        }
    }
}