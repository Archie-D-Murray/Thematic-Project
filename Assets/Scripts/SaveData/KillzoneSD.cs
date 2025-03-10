using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data {
    [Serializable]
    public class KillzoneSD {
        public Vector3Int Position;

        public KillzoneSD(Vector3Int position) {
            Position = position;
        }   
    }
}
