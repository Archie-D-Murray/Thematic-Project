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
                        ui.Readout.sprite = manager.Checkmark;
                        return true;
                    } else {
                        return false;
                    }
                case TutorialType.PlaceTile:
                    if (manager.PlacedTiles.Count >= manager.MinTileTypes) {
                        ui.Readout.fillAmount = 1.0f;
                        ui.ReadoutText.text = $"{manager.PlacedTiles.Count} / {manager.MinTileTypes}";
                        ui.Panel.color = Color.gray;
                        return true;
                    } else {
                        ui.Readout.fillAmount = Mathf.Clamp01((float)manager.PlacedTiles.Count / (float)manager.MinTileTypes);
                        return false;
                    }
                case TutorialType.PlaceObstacle:
                    if (manager.PlacedObstacles.Count >= manager.MinObstacleTypes) {
                        ui.Readout.fillAmount = 1.0f;
                        ui.ReadoutText.text = $"{manager.PlacedObstacles.Count} / {manager.MinObstacleTypes}";
                        ui.Panel.color = Color.gray;
                        return true;
                    } else {
                        ui.Readout.fillAmount = Mathf.Clamp01((float)manager.PlacedObstacles.Count / (float)manager.MinObstacleTypes);
                        return false;
                    }
                case TutorialType.RemoveObstacle:
                    if (manager.HasDestroyedObstacle) {
                        ui.Readout.sprite = manager.Checkmark;
                        ui.Panel.color = Color.gray;
                        return true;
                    } else {
                        return false;
                    }
                case TutorialType.PickupObstacle:
                    if (manager.HasPickedUpObstacle) {
                        ui.Readout.sprite = manager.Checkmark;
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