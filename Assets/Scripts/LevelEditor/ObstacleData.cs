using UnityEngine;
using System;

namespace LevelEditor {

    public enum ObstacleType { Door, Laser, Platform }

    [CreateAssetMenu(menuName = "Obstacle")]
    public class ObstacleData : ScriptableObject {
        public GameObject Prefab;
        public KeyCode Key;
        public Sprite Sprite;
        public ObstacleType Obstacle;

        public Type Type() {
            return Obstacle switch {
                ObstacleType.Laser => typeof(Door),
                ObstacleType.Platform => typeof(Door),
                _ => typeof(Door)
            };
        }
    }
}