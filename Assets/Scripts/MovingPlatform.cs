using System;
using System.Collections;
using System.Collections.Generic;

using Data;

using LevelEditor;

using Tags.Obstacle;

using UnityEngine;
using UnityEngine.UIElements;

public class MovingPlatform : Placeable {
    private Rigidbody2D platform;
    [SerializeField] private Transform pos1, pos2;
    private SpriteRenderer[] _pointRenderers;
    private Vector3 currTarget;
    [SerializeField] private float speed;
    [SerializeField] private bool reset;

    protected override Transform[] GetMoveables() {
        if (!pos1 || !pos2 || !platform) {
            GetPositions();
        }
        if (_pointRenderers == null || _pointRenderers.Length < 1) {
            _pointRenderers = new SpriteRenderer[2] { pos1.GetComponent<SpriteRenderer>(), pos2.GetComponent<SpriteRenderer>() };
        }
        return new Transform[] { platform.transform, pos1, pos2 };
    }

    // Start is called before the first frame update
    void Start() {
        GetPositions();
        _moveables = GetMoveables();
        currTarget = pos1.position;
        OnPlaceStart += PlacementStart;
        OnPlaceFinish += PlacementFinish;
    }

    private void PlacementStart() {
        foreach (SpriteRenderer renderer in _pointRenderers) {
            renderer.Fade(Color.clear, Color.white, 0.5f, this);
        }
    }

    private void PlacementFinish() {
        currTarget = pos1.position;
        foreach (SpriteRenderer renderer in _pointRenderers) {
            renderer.Fade(Color.white, Color.clear, 0.5f, this);
        }
    }

    void GetPositions() {
        platform = GetComponentInChildren<Rigidbody2D>();
        foreach (Transform child in transform) {
            if (child.gameObject.HasComponent<FirstPosition>()) {
                pos1 = child;
            } else if (child.gameObject.HasComponent<SecondPosition>()) {
                pos2 = child;
            }
        }
    }

    // Update is called once per frame
    void Update() {
        if (!_playing) { return; }
        platform.transform.position = Vector2.MoveTowards(platform.transform.position, currTarget, speed * Time.deltaTime);

        if (Vector2.Distance(pos1.position, platform.transform.position) <= 0.1) {
            currTarget = pos2.position;
        } else if (Vector2.Distance(pos2.position, platform.transform.position) <= 0.1) {
            if (reset) {
                platform.transform.position = pos1.position;
            } else {
                currTarget = pos1.position;
            }
        }
    }

    public void LoadSaveData(PlatformData data) {
        GetPositions();
        platform.position = data.CurrentPosition;
        pos1.position = data.Pos1;
        pos2.position = data.Pos2;
    }

    public PlatformData ToSaveData() {
        return new PlatformData(_initialPosition, pos1.position, pos2.position);
    }
}