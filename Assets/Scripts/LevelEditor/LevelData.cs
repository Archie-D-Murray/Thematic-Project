using System.Collections.Generic;
namespace Data {
    public class LevelData {
        public List<TileData> TilemapData;
        public List<DoorData> DoorData;
        public List<PlatformData> PlatformData;
        public List<PlatformData> DeathPlatformData;
        public List<PatrolEnemyData> PatrolEnemies;
        public List<FlyingEnemyData> FlyingEnemies;
        public List<LaserData> Lasers;

        public LevelData() {
            TilemapData = new List<TileData>();
            DoorData = new List<DoorData>();
            PlatformData = new List<PlatformData>();
            DeathPlatformData = new List<PlatformData>();
            PatrolEnemies = new List<PatrolEnemyData>();
            FlyingEnemies = new List<FlyingEnemyData>();
            Lasers = new List<LaserData>();
        }
    }
}