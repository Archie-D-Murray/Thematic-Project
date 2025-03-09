using System.Collections.Generic;
namespace Data {
    public class LevelData {
        public List<TileData> TilemapData;
        public List<DoorData> DoorData;
        public List<PlatformData> PlatformData;

        public LevelData() {
            TilemapData = new List<TileData>();
            DoorData = new List<DoorData>();
            PlatformData = new List<PlatformData>();
        }
    }
}