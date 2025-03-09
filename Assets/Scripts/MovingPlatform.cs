using System;
using System.Collections;
using System.Collections.Generic;

using Data;

using LevelEditor;

using Tags.Obstacle;

using UnityEngine;
using UnityEngine.UIElements;

public class MovingPlatform : Placeable {
    Rigidbody2D platform;
    [SerializeField] Transform pos1, pos2;
    Vector3 currTarget;
    [SerializeField] private float speed;
    [SerializeField] private bool reset;

    protected override Transform[] GetMoveables() {
        if (!pos1 || !pos2) {
            GetPositions();
        }
        return new Transform[] { transform, pos1, pos2 };
    }

    // Start is called before the first frame update
    void Start() {
        platform = GetComponentInChildren<Rigidbody2D>();
        GetPositions();
        InitReferences();
        currTarget = pos1.position;
        OnPlaceFinish += () => currTarget = pos1.position;
    }

    void GetPositions() {
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
        if (_placing) { return; }
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
        transform.position = data.CurrentPosition;
        pos1.position = data.Pos1;
        pos2.position = data.Pos2;
    }

    public PlatformData ToSaveData() {
        return new PlatformData(transform.position, pos1.position, pos2.position);
    }
}