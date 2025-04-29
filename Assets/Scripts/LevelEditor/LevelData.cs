using System;
using System.Collections.Generic;

using UnityEngine;
namespace Data {
    [Serializable]
    public class LevelData {
        public string Name;
        public List<TileData> ForegroundData;
        public List<TileData> BackgroundData;
        public List<DoorData> DoorData;
        public List<PlatformData> PlatformData;
        public List<PlatformData> DeathPlatformData;
        public List<PatrolEnemyData> PatrolEnemies;
        public List<PatrolEnemyData> SlowedEnemies;
        public List<StaticEnemyData> FlyingEnemies;
        public List<StaticEnemyData> TurretEnemies;
        public List<LaserData> Lasers;
        public SpawnPointData SpawnPoint;
        public EndPointData EndPoint;

        public LevelData(string name) {
            Name = name;
            ForegroundData = new List<TileData>();
            BackgroundData = new List<TileData>();
            DoorData = new List<DoorData>();
            PlatformData = new List<PlatformData>();
            DeathPlatformData = new List<PlatformData>();
            PatrolEnemies = new List<PatrolEnemyData>();
            SlowedEnemies = new List<PatrolEnemyData>();
            FlyingEnemies = new List<StaticEnemyData>();
            TurretEnemies = new List<StaticEnemyData>();
            Lasers = new List<LaserData>();
            SpawnPoint = new SpawnPointData(Vector3.zero);
            EndPoint = new EndPointData(Vector3.zero);
        }
    }
}