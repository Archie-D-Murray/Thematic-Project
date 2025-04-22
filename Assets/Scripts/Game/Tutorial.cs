using UnityEngine;

namespace Game {

    public enum TutorialType { Attack, PlaceTile, PlaceObstacle, RemoveObstacle, PickupObstacle }

    [CreateAssetMenu(menuName = "Tutorial")]
    public class Tutorial : ScriptableObject {
        public string Task;
        public TutorialType Type;
        public GameObject Panel;

        public bool CheckComplete(TutorialManager manager) {
            switch (Type) {
                case TutorialType.Attack:
                    return manager.KilledEnemy;
                case TutorialType.PlaceTile:
                    return manager.PlacedTiles.Count >= manager.MinTileTypes;
                case TutorialType.PlaceObstacle:
                    return manager.PlacedObstacles.Count >= manager.MinPlacedTypes;
                case TutorialType.RemoveObstacle:
                    return manager.HasDestroyedObstacle;
                case TutorialType.PickupObstacle:
                    return manager.HasPickedUpObstacle;
                default:
                    return false;
            }
        }
    }
}