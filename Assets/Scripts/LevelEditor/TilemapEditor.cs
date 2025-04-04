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
            public Tilemap _tilemap;
            public int _selected = 0;
            public TileBase[] _tileAssets;
            public Tile[] _tiles;
            public Dictionary<Button, int> _lookup = new Dictionary<Button, int>();
            public Dictionary<Vector3Int, int> _levelTiles = new Dictionary<Vector3Int, int>();

            public void AddTile(Vector3Int position) {
                _tilemap.SetTile(position, _tileAssets[_selected]);
                _levelTiles[position] = _selected;
            }

            public void RemoveTile(Vector3Int position) {
                _tilemap.SetTile(position, null);
                _levelTiles.Remove(position);
            }

            public void IncrementIndex() {
                _selected = ++_selected % _tileAssets.Length;
            }

            public void DecrementIndex() {
                _selected--;
                if (_selected < 0) {
                    _selected = _tileAssets.Length - 1;
                }
            }

            public bool IsValid() {
                return _tileAssets.Length == _tiles.Length;
            }
        }

        [SerializeField] private TilemapData[] _tilemapData = new TilemapData[2];
        [SerializeField] private int _index = 0;

        [SerializeField] public TileBase _errorTile;

        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private GameObject _indicatorPrefab;
        [SerializeField] private GameObject _selectionPrefab;
        [SerializeField] private CanvasGroup _tilemapSelection;

        [SerializeField] private KeyCode _cycleForward;
        [SerializeField] private KeyCode _cycleBackward;

        private SpriteRenderer _indicator;
        private Image _selection;
        private Button _toggle;
        private Transform _tiles;

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

        private void ToggleTilemap() {
            _index = 1 - _index;
            PopulateUI();
        }

        public void PopulateUI() {
            if (_tiles.childCount != 0) {
                foreach (Transform child in _tiles) {
                    Destroy(child.gameObject);
                }
                _current._lookup.Clear();
            }
            for (int i = 0; i < _current._tileAssets.Length; i++) {
                Button button = Instantiate(_tilePrefab, _tiles).GetComponent<Button>();
                _current._lookup.Add(button, i);
                button.onClick.AddListener(() => {
                    _current._selected = _current._lookup[button];
                    _indicator.sprite = _current._tiles[_current._lookup[button]].sprite;
                    _selection.transform.position = button.transform.position;
                });
                button.GetComponent<Image>().sprite = _current._tiles[i].sprite;
                if (i == _current._selected) { // GridLayout only updates on next frame...
                    StartCoroutine(SetSelectionPosition(button.transform));
                }
            }
        }

        private IEnumerator SetSelectionPosition(Transform transform) {
            yield return Yielders.WaitForEndOfFrame;
            _selection.transform.position = transform.position;
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
                _current.AddTile(position);
            } else if (Input.GetMouseButton(1) && !UIManager.Instance.IsHovered()) {
                Vector3Int position = Vector3Int.FloorToInt(mousePosition);
                _current.RemoveTile(position);
            }

            if (Input.GetKeyDown(_cycleForward)) {
                _current.IncrementIndex();
            }
            if (Input.GetKeyDown(_cycleBackward)) {
                _current.DecrementIndex();
            }
        }

        public void OnSave(ref LevelData data) {
            foreach (TilemapData tilemap in _tilemapData) {
                if (tilemap.Type == TilemapType.Foreground) {
                    foreach ((Vector3Int position, int ID) tile in tilemap._levelTiles.Select(kvp => (kvp.Key, kvp.Value))) {
                        data.ForegroundData.Add(new TileData(tile.ID, tile.position));
                    }
                } else if (tilemap.Type == TilemapType.Background) {
                    foreach ((Vector3Int position, int ID) tile in tilemap._levelTiles.Select(kvp => (kvp.Key, kvp.Value))) {
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
                tilemap._tilemap.ClearAllTiles();
                tilemap._levelTiles.Clear();
                if (tilemap.Type == TilemapType.Foreground) {
                    foreach (TileData tile in data.ForegroundData) {
                        if (tile.ID >= tilemap._tileAssets.Length) {
                            Debug.LogWarning($"Tile of ID: {tile.ID} could not be parsed as it is not present in known tiles", this);
                            tilemap._tilemap.SetTile(tile.Position, _errorTile);
                        } else {
                            tilemap._tilemap.SetTile(tile.Position, tilemap._tileAssets[tile.ID]);
                            tilemap._levelTiles[tile.Position] = tile.ID;
                        }
                    }
                } else if (tilemap.Type == TilemapType.Background) {
                    foreach (TileData tile in data.BackgroundData) {
                        if (tile.ID >= tilemap._tileAssets.Length) {
                            Debug.LogWarning($"Tile of ID: {tile.ID} could not be parsed as it is not present in known tiles", this);
                            tilemap._tilemap.SetTile(tile.Position, _errorTile);
                        } else {
                            tilemap._tilemap.SetTile(tile.Position, tilemap._tileAssets[tile.ID]);
                            tilemap._levelTiles[tile.Position] = tile.ID;
                        }
                    }
                } else {
                    Debug.LogWarning($"Encounted unknown tilemap type: {tilemap.Type}, skipping...");
                    continue;
                }
            }
        }

        private void AddExistingTiles() {
            foreach (TilemapData data in _tilemapData) {
                Dictionary<TileBase, int> lookup = new Dictionary<TileBase, int>();
                for (int i = 0; i < data._tileAssets.Length; i++) {
                    lookup.Add(data._tileAssets[i], i);
                }
                for (int y = data._tilemap.origin.y; y < data._tilemap.size.y; y++) {
                    for (int x = data._tilemap.origin.x; x < data._tilemap.size.x; x++) {
                        Vector3Int position = new Vector3Int(x, y, 0);
                        TileBase tile = data._tilemap.GetTile(position);
                        if (tile != null && lookup.TryGetValue(tile, out int index)) {
                            data._levelTiles[position] = lookup[tile];
                        }
                    }
                }
            }
        }
    }
}