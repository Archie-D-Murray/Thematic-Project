using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

using UI;

using Data;
using TileData = Data.TileData;
using Utilities;
using System;
using System.Collections;

namespace LevelEditor {
    public class TilemapEditor : MonoBehaviour, ISerialize {

        public enum TilemapType { Foreground, Background }

        [Serializable]
        public class TilemapData {
            public TilemapType Type;
            public Tilemap Tilemap;
            public int Selected = 0;
            public TileBase[] TileAssets;
            public Tile[] Tiles;
            public Dictionary<Button, int> Lookup = new Dictionary<Button, int>();
            public Vector3[] ButtonPositions;
            public Dictionary<Vector3Int, int> LevelTiles = new Dictionary<Vector3Int, int>();

            public void SetTile(Vector3Int position) {
                Tilemap.SetTile(position, TileAssets[Selected]);
                LevelTiles[position] = Selected;
            }

            public void RemoveTile(Vector3Int position) {
                Tilemap.SetTile(position, null);
                LevelTiles.Remove(position);
            }

            public void IncrementIndex() {
                Selected = ++Selected % TileAssets.Length;
            }

            public void DecrementIndex() {
                Selected--;
                if (Selected < 0) {
                    Selected = TileAssets.Length - 1;
                }
            }

            public bool IsValid() {
                return TileAssets.Length == Tiles.Length;
            }

            public void PopulateButtonPositions() {
                if (ButtonPositions != null && ButtonPositions.Length != 0) { return; }
                ButtonPositions = new Vector3[Tiles.Length];
                int i = 0;
                foreach (Button button in Lookup.Keys) {
                    ButtonPositions[i] = button.transform.position;
                    i++;
                }
            }

            public Vector3 ButtonPosition() {
                return ButtonPositions[Selected];
            }

            public Sprite Sprite() {
                return Tiles[Selected].sprite;
            }

            public bool Different(Vector3Int position) {
                return !(LevelTiles.ContainsKey(position) && LevelTiles[position] == Selected);
            }
        }

        [SerializeField] private TilemapData[] _tilemapData = new TilemapData[2];
        [SerializeField] private int _index = 0;

        [SerializeField] public TileBase _errorTile;

        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private GameObject _indicatorPrefab;
        [SerializeField] private GameObject _selectionPrefab;

        [SerializeField] private KeyCode _cycleForward = KeyCode.RightBracket;
        [SerializeField] private KeyCode _cycleBackward = KeyCode.LeftBracket;
        [SerializeField] private KeyCode _toggleTilemap = KeyCode.Space;

        [SerializeField] private CanvasGroup _tilemapSelection;
        [SerializeField] private PolygonCollider2D _cameraBounds;

        private SpriteRenderer _indicator;
        private Image _selection;
        private Button _toggle;
        private Transform _tiles;

        public Action<TilemapType> OnTilePlace;

        private TilemapData _current => _tilemapData[_index];

        public float Alpha => _tilemapSelection.alpha;

        private void Start() {
            if (!_errorTile) {
                // We cannot safely parse tiledata
                Debug.LogError("Error tile not assigned", this);
                enabled = false;
                return;
            }
            _tiles = GetComponentInChildren<GridLayoutGroup>().transform;
            _toggle = GetComponentInChildren<Button>();
            _tilemapSelection = GetComponent<CanvasGroup>();
            _indicator = Instantiate(_indicatorPrefab, Vector3.zero, Quaternion.identity).GetComponent<SpriteRenderer>();
            _selection = Instantiate(_selectionPrefab, transform.parent).GetComponent<Image>();
            foreach (TilemapData data in _tilemapData) {
                if (!data.IsValid()) {
                    Debug.LogError("Tile Assets and Tiles not same size?", this);
                    enabled = false;
                    return;
                }
            }
            PopulateUI();
            _toggle.onClick.AddListener(ToggleTilemap);
            AddExistingTiles();
            Close();
        }

        private void UpdateBounds() {
            Vector3Int min = _tilemapData[0].Tilemap.cellBounds.min;
            Vector3Int max = _tilemapData[0].Tilemap.cellBounds.max;
            foreach (TilemapData data in _tilemapData) {
                Debug.Log($"{data.Type}: Bounds: min: {data.Tilemap.cellBounds.min}, max: {data.Tilemap.cellBounds.max}");
                min = Vector3Int.Min(data.Tilemap.cellBounds.min, min);
                max = Vector3Int.Max(data.Tilemap.cellBounds.max, max);
            }
            Debug.Log($"Found bounds: min: {min}, max {max}");
            _cameraBounds.SetPath(0, new Vector2[] {
                new Vector2(min.x, min.y),
                new Vector2(min.x, max.y),
                new Vector2(max.x, max.y),
                new Vector2(max.x, min.y),
            });
        }

        private void ToggleTilemap() {
            _index = 1 - _index;
            PopulateUI();
        }

        public void PopulateUI() {
            if (_tiles.childCount != 0) {
                foreach (Transform child in _tiles) {
                    Destroy(child.gameObject);
                }
                _current.Lookup.Clear();
            }
            for (int i = 0; i < _current.TileAssets.Length; i++) {
                Button button = Instantiate(_tilePrefab, _tiles).GetComponent<Button>();
                _current.Lookup.Add(button, i);
                button.onClick.AddListener(() => {
                    _current.Selected = _current.Lookup[button];
                    _indicator.sprite = _current.Tiles[_current.Lookup[button]].sprite;
                    _selection.transform.position = button.transform.position;
                });
                button.GetComponent<Image>().sprite = _current.Tiles[i].sprite;
            }
            StartCoroutine(SetSelectionPosition());
        }

        private IEnumerator SetSelectionPosition() {
            yield return Yielders.WaitForEndOfFrame;
            _current.PopulateButtonPositions();
            _indicator.sprite = _current.Sprite();
            _selection.transform.position = _current.ButtonPosition();
        }

        public void Close() {
            _indicator.gameObject.SetActive(false);
            _selection.gameObject.SetActive(false);
            _tilemapSelection.FadeCanvas(2.0f, true, this);
        }

        public void Open() {
            _indicator.gameObject.SetActive(true);
            _selection.gameObject.SetActive(true);
            _tilemapSelection.FadeCanvas(2.0f, false, this);
        }

        private void Update() {
            if (_tilemapSelection.alpha != 1.0f) { return; }
            Vector2 mousePosition = Helpers.Instance.TileMapMousePosition;
            _indicator.transform.position = mousePosition;
            if (Input.GetMouseButton(0) && !UIManager.Instance.IsHovered()) {
                Vector3Int position = Vector3Int.FloorToInt(mousePosition);
                if (_current.Different(position)) {
                    OnTilePlace?.Invoke(_current.Type);
                }
                _current.SetTile(position);
            } else if (Input.GetMouseButton(1) && !UIManager.Instance.IsHovered()) {
                Vector3Int position = Vector3Int.FloorToInt(mousePosition);
                _current.RemoveTile(position);
            }

            if (Input.GetKeyDown(_cycleForward)) {
                Debug.Log("Cycling forward");
                _current.IncrementIndex();
                _selection.transform.position = _current.ButtonPosition();
                _indicator.sprite = _current.Sprite();
            }
            if (Input.GetKeyDown(_cycleBackward)) {
                Debug.Log("Cycling backward");
                _current.DecrementIndex();
                _selection.transform.position = _current.ButtonPosition();
                _indicator.sprite = _current.Sprite();
            }
            if (Input.GetKeyDown(_toggleTilemap)) {
                ToggleTilemap();
            }
        }

        public void OnSave(ref LevelData data) {
            foreach (TilemapData tilemap in _tilemapData) {
                if (tilemap.Type == TilemapType.Foreground) {
                    foreach ((Vector3Int position, int ID) tile in tilemap.LevelTiles.Select(kvp => (kvp.Key, kvp.Value))) {
                        data.ForegroundData.Add(new TileData(tile.ID, tile.position));
                    }
                } else if (tilemap.Type == TilemapType.Background) {
                    foreach ((Vector3Int position, int ID) tile in tilemap.LevelTiles.Select(kvp => (kvp.Key, kvp.Value))) {
                        data.BackgroundData.Add(new TileData(tile.ID, tile.position));
                    }
                } else {
                    Debug.LogWarning($"Encounted unknown tilemap type: {tilemap.Type}, skipping...");
                    continue;
                }
            }
        }

        public void OnLoad(LevelData data) {
            foreach (TilemapData tilemap in _tilemapData) {
                tilemap.Tilemap.ClearAllTiles();
                tilemap.LevelTiles.Clear();
                if (tilemap.Type == TilemapType.Foreground) {
                    foreach (TileData tile in data.ForegroundData) {
                        if (tile.ID >= tilemap.TileAssets.Length) {
                            Debug.LogWarning($"Tile of ID: {tile.ID} could not be parsed as it is not present in known tiles", this);
                            tilemap.Tilemap.SetTile(tile.Position, _errorTile);
                        } else {
                            tilemap.Tilemap.SetTile(tile.Position, tilemap.TileAssets[tile.ID]);
                            tilemap.LevelTiles[tile.Position] = tile.ID;
                        }
                    }
                } else if (tilemap.Type == TilemapType.Background) {
                    foreach (TileData tile in data.BackgroundData) {
                        if (tile.ID >= tilemap.TileAssets.Length) {
                            Debug.LogWarning($"Tile of ID: {tile.ID} could not be parsed as it is not present in known tiles", this);
                            tilemap.Tilemap.SetTile(tile.Position, _errorTile);
                        } else {
                            tilemap.Tilemap.SetTile(tile.Position, tilemap.TileAssets[tile.ID]);
                            tilemap.LevelTiles[tile.Position] = tile.ID;
                        }
                    }
                } else {
                    Debug.LogWarning($"Encounted unknown tilemap type: {tilemap.Type}, skipping...");
                    continue;
                }
            }
            UpdateBounds();
        }

        private void AddExistingTiles() {
            foreach (TilemapData data in _tilemapData) {
                Dictionary<TileBase, int> lookup = new Dictionary<TileBase, int>();
                for (int i = 0; i < data.TileAssets.Length; i++) {
                    lookup.Add(data.TileAssets[i], i);
                }
                for (int y = data.Tilemap.origin.y; y < data.Tilemap.size.y; y++) {
                    for (int x = data.Tilemap.origin.x; x < data.Tilemap.size.x; x++) {
                        Vector3Int position = new Vector3Int(x, y, 0);
                        TileBase tile = data.Tilemap.GetTile(position);
                        if (tile != null && lookup.TryGetValue(tile, out int index)) {
                            data.LevelTiles[position] = lookup[tile];
                        }
                    }
                }
            }
        }
    }
}