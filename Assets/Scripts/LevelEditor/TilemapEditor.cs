using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

using UI;

using Data;
using TileData = Data.TileData;
using Utilities;

namespace LevelEditor {
    public class TilemapEditor : MonoBehaviour, ISerialize {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private int _selected = 0;
        [SerializeField] private TileBase[] _tileAssets;
        [SerializeField] private Tile[] _tiles;
        [SerializeField] private TileBase _errorTile;
        [SerializeField] private CanvasGroup _tilemapSelection;
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private GameObject _indicatorPrefab;
        [SerializeField] private GameObject _selectionPrefab;

        private SpriteRenderer _indicator;
        private Image _selection;

        private Dictionary<Button, int> _lookup = new Dictionary<Button, int>();
        private Dictionary<Vector3Int, int> _levelTiles = new Dictionary<Vector3Int, int>();

        private void Start() {
            if (!_errorTile) {
                // We cannot safely parse tiledata
                Debug.LogError("Error tile not assigned", this);
                enabled = false;
                return;
            }
            _tilemapSelection = GetComponent<CanvasGroup>();
            _indicator = Instantiate(_indicatorPrefab, Vector3.zero, Quaternion.identity).GetComponent<SpriteRenderer>();
            _selection = Instantiate(_selectionPrefab, transform.parent).GetComponent<Image>();
            if (_tileAssets.Length != _tiles.Length) {
                Debug.LogError("Tile Assets and Tiles not same size?", this);
                enabled = false;
                return;
            }
            for (int i = 0; i < _tileAssets.Length; i++) {
                Button button = Instantiate(_tilePrefab, transform).GetComponent<Button>();
                _lookup.Add(button, i);
                button.onClick.AddListener(() => {
                    _selected = _lookup[button];
                    _indicator.sprite = _tiles[_lookup[button]].sprite;
                    _selection.transform.position = button.transform.position;
                });
                button.GetComponent<Image>().sprite = _tiles[i].sprite;
            }
            _selection.rectTransform.position = new Vector2(Screen.width * 2, Screen.height * 2);
            AddExistingTiles();
            Disable();
        }

        private void Disable() {
            _indicator.gameObject.SetActive(false);
            _selection.gameObject.SetActive(false);
            _tilemapSelection.FadeCanvas(0.5f, true, this);
        }

        private void Enable() {
            _indicator.gameObject.SetActive(true);
            _selection.gameObject.SetActive(true);
            _tilemapSelection.FadeCanvas(0.5f, false, this);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.F5)) {
                if (_tilemapSelection.alpha > 0.0f) {
                    Disable();
                } else {
                    Enable();
                }
            }
            if (_tilemapSelection.alpha != 1.0f) { return; }
            Vector2 mousePosition = Helpers.Instance.TileMapMousePosition;
            _indicator.transform.position = mousePosition;
            if (Input.GetMouseButton(0) && !UIManager.Instance.IsHovered()) {
                Vector3Int position = Vector3Int.FloorToInt(mousePosition);
                _tilemap.SetTile(position, _tileAssets[_selected]);
                _levelTiles[position] = _selected;
            } else if (Input.GetMouseButton(1) && !UIManager.Instance.IsHovered()) {
                Vector3Int position = Vector3Int.FloorToInt(mousePosition);
                _tilemap.SetTile(position, null);
                if (_levelTiles.ContainsKey(position)) {
                    _levelTiles.Remove(position);
                }
            }
        }

        public void OnSave(ref LevelData data) {
            foreach ((Vector3Int position, int ID) tile in _levelTiles.Select(kvp => (kvp.Key, kvp.Value))) {
                data.TilemapData.Add(new TileData(tile.ID, tile.position));
            }
        }

        public void OnLoad(LevelData data) {
            _tilemap.ClearAllTiles();
            _levelTiles.Clear();
            foreach (TileData tile in data.TilemapData) {
                if (tile.ID >= _tileAssets.Length) {
                    Debug.LogWarning($"Tile of ID: {tile.ID} could not be parsed as it is not present in known tiles", this);
                    _tilemap.SetTile(tile.Position, _errorTile);
                } else {
                    _tilemap.SetTile(tile.Position, _tileAssets[tile.ID]);
                    _levelTiles[tile.Position] = tile.ID;
                }
            }
        }

        private void AddExistingTiles() {
            Dictionary<TileBase, int> lookup = new Dictionary<TileBase, int>();
            for (int i = 0; i < _tileAssets.Length; i++) {
                lookup.Add(_tileAssets[i], i);
            }
            for (int y = _tilemap.origin.y; y < _tilemap.size.y; y++) {
                for (int x = _tilemap.origin.x; x < _tilemap.size.x; x++) {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    TileBase tile = _tilemap.GetTile(position);
                    if (tile != null && lookup.TryGetValue(tile, out int index)) {
                        _levelTiles[position] = lookup[tile];
                    }
                }
            }
        }
    }
}