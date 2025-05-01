using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraBoundsManager : MonoBehaviour {
    [SerializeField] private Tilemap[] _tilemaps;
    [SerializeField] private PolygonCollider2D _cameraBounds;

    private void Start() {
        UpdateBounds();
    }

    public void UpdateBounds() {
        Vector3Int min = _tilemaps[0].cellBounds.min;
        Vector3Int max = _tilemaps[0].cellBounds.max;
        foreach (Tilemap tilemap in _tilemaps) {
            min = Vector3Int.Min(tilemap.cellBounds.min, min);
            max = Vector3Int.Max(tilemap.cellBounds.max, max);
        }
        Debug.Log($"Found bounds: min: {min}, max {max}");
        _cameraBounds.SetPath(0, new Vector2[] {
                new Vector2(min.x, min.y),
                new Vector2(min.x, max.y),
                new Vector2(max.x, max.y),
                new Vector2(max.x, min.y),
            });
    }
}