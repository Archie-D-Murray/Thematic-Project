using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

using UI;

namespace LevelEditor {
    public class TilemapEditor : MonoBehaviour {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private int _selected = 0;
        [SerializeField] private TileBase[] _tileAssets;
        [SerializeField] private Tile[] _tiles;
        [SerializeField] private CanvasGroup _tilemapSelection;
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private GameObject _indicatorPrefab;
        [SerializeField] private GameObject _selectionPrefab;

        private SpriteRenderer _indicator;
        private Image _selection;

        private Dictionary<Button, int> _lookup = new Dictionary<Button, int>();

        private void Start() {
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
        }

        private void Update() {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            _indicator.transform.position = Vector3Int.FloorToInt(mousePosition) + new Vector3(0.5f, 0.5f, 0);
            if (Input.GetMouseButton(0) && !UIManager.Instance.IsHovered()) {
                _tilemap.SetTile(Vector3Int.FloorToInt(mousePosition), _tileAssets[_selected]);
            }
        }
    }
}