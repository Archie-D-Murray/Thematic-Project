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
                platform.InitReferences();
                platform.LoadSaveData(platformData);
                platform.EnterPlayMode();
            }
            foreach (PlatformData platformData in data.DeathPlatformData) {
                MovingPlatform platform = Instantiate(_obstacleLookup[ObstacleType.DeathPlatform].Prefab).GetComponent<MovingPlatform>();
                platform.InitReferences();
                platform.LoadSaveData(platformData);
                platform.EnterPlayMode();
            }
            foreach (PatrolEnemyData patrolEnemy in data.PatrolEnemies) {
                PatrolEnemy patrol = Instantiate(_obstacleLookup[ObstacleType.PatrolEnemy].Prefab).GetComponentInChildren<PatrolEnemy>();
                patrol.InitReferences();
                patrol.LoadSaveData(patrolEnemy);
                patrol.EnterPlayMode();
            }
            foreach (PatrolEnemyData slowEnemy in data.SlowedEnemies) {
                PatrolEnemy slow = Instantiate(_obstacleLookup[ObstacleType.SlowEnemy].Prefab).GetComponentInChildren<PatrolEnemy>();
                slow.InitReferences();
                slow.LoadSaveData(slowEnemy);
                slow.EnterPlayMode();
            }
            foreach (StaticEnemyData flyingEnemy in data.FlyingEnemies) {
                FlyingEnemy flying = Instantiate(_obstacleLookup[ObstacleType.FlyingEnemy].Prefab).GetComponent<FlyingEnemy>();
                flying.InitReferences();
                flying.LoadSaveData(flyingEnemy);
                flying.EnterPlayMode();
            }
            foreach (StaticEnemyData turretEnemy in data.TurretEnemies) {
                TurretEnemy turret = Instantiate(_obstacleLookup[ObstacleType.TurretEnemy].Prefab).GetComponent<TurretEnemy>();
                turret.InitReferences();
                turret.LoadSaveData(turretEnemy);
                turret.EnterPlayMode();
            }
            foreach (LaserData laserData in data.Lasers) {
                Laser laser = Instantiate(_obstacleLookup[ObstacleType.Laser].Prefab).GetComponent<Laser>();
                laser.InitReferences();
                laser.LoadSaveData(laserData);
                laser.EnterPlayMode();
            }
            if (data.SpawnPoint != null) {
                SpawnPoint spawnPoint = Instantiate(_obstacleLookup[ObstacleType.SpawnPoint].Prefab).GetComponent<SpawnPoint>();
                spawnPoint.InitReferences();
                spawnPoint.LoadSaveData(data.SpawnPoint);
                spawnPoint.EnterPlayMode();
            }
            if (data.EndPoint != null) {
                EndPoint endPoint = Instantiate(_obstacleLookup[ObstacleType.EndPoint].Prefab).GetComponent<EndPoint>();
                endPoint.InitReferences();
                endPoint.LoadSaveData(data.EndPoint);
                endPoint.EnterPlayMode();
            }
        }

        public void OnSave(ref LevelData data) { }
    }
}