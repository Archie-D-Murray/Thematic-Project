using System;

using UnityEngine;
namespace Data {
    [Serializable]
    public class TileData {
        public int ID;
        public Vector3Int Position;

        public TileData(int ID, Vector3Int position) {
            this.ID = ID;
            Position = position;
        }
    }
}