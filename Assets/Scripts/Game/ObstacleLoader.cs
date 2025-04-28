using System.Collections.Generic;

using Data;

using LevelEditor;

using UnityEngine;

namespace Game {
    public class ObstacleLoader : MonoBehaviour, ISerialize {

        [SerializeField] private ObstacleData[] _obstacles;

        private Dictionary<ObstacleType, ObstacleData> _obstacleLookup = new Dictionary<ObstacleType, ObstacleData>();

        public void OnLoad(LevelData data) {
            foreach (ObstacleData obstacleData in _obstacles) {
                _obstacleLookup.Add(obstacleData.Obstacle, obstacleData);
            }
            foreach (DoorData doorData in data.DoorData) {
                Door door = Instantiate(_obstacleLookup[ObstacleType.Door].Prefab).GetComponent<Door>();
                door.LoadSaveData(doorData);
                door.InitReferences();
                door.EnterPlayMode();
            }
            foreach (PlatformData platformData in data.PlatformData) {
                MovingPlatform platform = Instantiate(_obstacleLookup[ObstacleType.Platform].Prefab).GetComponent<MovingPlatform>();
                platform.LoadSaveData(platformData);
                platform.InitReferences();
                platform.EnterPlayMode();
            }
            foreach (PlatformData platformData in data.DeathPlatformData) {
                MovingPlatform platform = Instantiate(_obstacleLookup[ObstacleType.DeathPlatform].Prefab).GetComponent<MovingPlatform>();
                platform.LoadSaveData(platformData);
                platform.InitReferences();
                platform.EnterPlayMode();
            }
            foreach (PatrolEnemyData patrolEnemy in data.PatrolEnemies) {
                PatrolEnemy patrol = Instantiate(_obstacleLookup[ObstacleType.PatrolEnemy].Prefab).GetComponentInChildren<PatrolEnemy>();
                patrol.LoadSaveData(patrolEnemy);
                patrol.InitReferences();
                patrol.EnterPlayMode();
            }
            foreach (PatrolEnemyData slowEnemy in data.SlowedEnemies) {
                PatrolEnemy slow = Instantiate(_obstacleLookup[ObstacleType.SlowEnemy].Prefab).GetComponentInChildren<PatrolEnemy>();
                slow.LoadSaveData(slowEnemy);
                slow.InitReferences();
                slow.EnterPlayMode();
            }
            foreach (StaticEnemyData flyingEnemy in data.FlyingEnemies) {
                FlyingEnemy flying = Instantiate(_obstacleLookup[ObstacleType.FlyingEnemy].Prefab).GetComponent<FlyingEnemy>();
                flying.LoadSaveData(flyingEnemy);
                flying.InitReferences();
                flying.EnterPlayMode();
            }
            foreach (StaticEnemyData turretEnemy in data.TurretEnemies) {
                TurretEnemy turret = Instantiate(_obstacleLookup[ObstacleType.TurretEnemy].Prefab).GetComponent<TurretEnemy>();
                turret.LoadSaveData(turretEnemy);
                turret.InitReferences();
                turret.EnterPlayMode();
            }
            foreach (LaserData laserData in data.Lasers) {
                Laser laser = Instantiate(_obstacleLookup[ObstacleType.Laser].Prefab).GetComponent<Laser>();
                laser.LoadSaveData(laserData);
                laser.InitReferences();
                laser.EnterPlayMode();
            }
            if (data.SpawnPoint != null) {
                SpawnPoint spawnPoint = Instantiate(_obstacleLookup[ObstacleType.SpawnPoint].Prefab).GetComponent<SpawnPoint>();
                spawnPoint.LoadSaveData(data.SpawnPoint);
                spawnPoint.InitReferences();
                spawnPoint.EnterPlayMode();
            }
            if (data.EndPoint != null) {
                EndPoint spawnPoint = Instantiate(_obstacleLookup[ObstacleType.EndPoint].Prefab).GetComponent<EndPoint>();
                spawnPoint.LoadSaveData(data.EndPoint);
                spawnPoint.InitReferences();
                spawnPoint.EnterPlayMode();
            }
        }

        public void OnSave(ref LevelData data) { }
    }
}