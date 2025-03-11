using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data {
    [Serializable]
    public class DoorSD {
        public bool Open;
        public Vector3Int Position;

        public DoorSD(bool open, Vector3Int position) {
            Open = open;
            Position = position;
        }   
    }
}