using System;

using UnityEngine;

namespace Game {

    public enum TutorialType { EditorMenu, PlayMode, Attack, PlaceTile, PlaceObstacle, RemoveObstacle, PickupObstacle }

    [CreateAssetMenu(menuName = "Tutorial")]
    public class Tutorial : ScriptableObject {
        public string Task;
        public TutorialType Type;
        public GameObject Panel;

        public bool CheckComplete(TutorialManager manager, TutorialUI ui) {
            switch (Type) {
                case TutorialType.Attack:
                    if (manager.KilledEnemy) {
                        ui.Panel.color = Color.gray;
                        return true;
                    } else {
                        return false;
                    }
                case TutorialType.PlaceTile:
                    if (manager.PlacedTiles.Count >= manager.MinTileTypes) {
                        ui._readout.fillAmount = 1.0f;
                        ui.Panel.color = Color.gray;
                        return true;
                    } else {
                        ui._readout.fillAmount = Mathf.Clamp01((float)manager.PlacedTiles.Count / (float)manager.MinTileTypes);
                        return false;
                    }
                case TutorialType.PlaceObstacle:
                    if (manager.PlacedObstacles.Count >= manager.MinObstacleTypes) {
                        ui._readout.fillAmount = 1.0f;
                        ui.Panel.color = Color.gray;
                        return true;
                    } else {
                        ui._readout.fillAmount = Mathf.Clamp01((float)manager.PlacedObstacles.Count / (float)manager.MinObstacleTypes);
                        return false;
                    }
                case TutorialType.RemoveObstacle:
                    if (manager.HasDestroyedObstacle) {
                        ui.Panel.color = Color.gray;
                        return true;
                    } else {
                        return false;
                    }
                case TutorialType.PickupObstacle:
                    if (manager.HasPickedUpObstacle) {
                        ui.Panel.color = Color.gray;
                        return true;
                    } else {
                        return false;
                    }
                default:
                    return false;
            }
        }

        public int GetSteps(TutorialManager manager) {
            return Type switch {
                TutorialType.PlaceObstacle => manager.MinObstacleTypes,
                TutorialType.PlaceTile => manager.MinTileTypes,
                _ => -1
            };
        }
    }
}