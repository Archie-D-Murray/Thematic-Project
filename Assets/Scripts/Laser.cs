using System;
using System.Collections;
using System.Collections.Generic;

using Data;

using LevelEditor;

using UnityEngine;

public class Laser : Placeable {
    [SerializeField] private float rotationSpeed = -360.0f;
    Transform _pivot;
    // Start is called before the first frame update
    void Start() {
        _pivot = transform.GetChild(0);
        InitReferences();
    }

    protected override Transform[] GetMoveables() {
        return new Transform[] { transform };
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!_playing) { return; }
        _pivot.rotation = Quaternion.AngleAxis(_pivot.rotation.eulerAngles.z + rotationSpeed * Time.fixedDeltaTime, Vector3.forward);
    }

    public LaserData ToSaveData() {
        return new LaserData(_initialPosition);
    }

    public void LoadSaveData(LaserData laserData) {
        transform.position = laserData.Position;
    }
}