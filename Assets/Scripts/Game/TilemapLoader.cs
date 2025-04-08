using System;
using System.Collections.Generic;

using Data;

using UnityEngine;
using UnityEngine.Tilemaps;

using static LevelEditor.TilemapEditor;

using TileData = Data.TileData;

namespace Game {
    public class TilemapLoader : MonoBehaviour, ISerialize {
        [Serializable]
        public class TilemapData {
            public TilemapType Type;
            public Tilemap _tilemap;
            public TileBase[] _tileAssets;

            public void AddTile(Data.TileData data) {
                _tilemap.SetTile(data.Position, _tileAssets[data.ID]);
            }
        }

        [SerializeField] private TilemapData[] _tilemapData = new TilemapData[2];

        public void OnLoad(LevelData data) {
            foreach (TilemapData tilemap in _tilemapData) {
                tilemap._tilemap.ClearAllTiles();
                if (tilemap.Type == TilemapType.Foreground) {
                    foreach (TileData tile in data.ForegroundData) {
                        if (tile.ID >= tilemap._tileAssets.Length) {
                            Debug.LogWarning($"Tile of ID: {tile.ID} could not be parsed as it is not present in known tiles", this);
                        } else {
                            tilemap.AddTile(tile);
                        }
                    }
                } else if (tilemap.Type == TilemapType.Background) {
                    foreach (TileData tile in data.BackgroundData) {
                        if (tile.ID >= tilemap._tileAssets.Length) {
                            Debug.LogWarning($"Tile of ID: {tile.ID} could not be parsed as it is not present in known tiles", this);
                        } else {
                            tilemap.AddTile(tile);
                        }
                    }
                } else {
                    Debug.LogWarning($"Encounted unknown tilemap type: {tilemap.Type}, skipping...");
                    continue;
                }
            }
        }

        public void OnSave(ref LevelData data) { }
    }
}