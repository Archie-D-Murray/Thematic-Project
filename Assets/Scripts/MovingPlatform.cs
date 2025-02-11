using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MovingPlatform : MonoBehaviour
{
    Rigidbody2D platform;
    Transform pos1, pos2, currTarget;
    [SerializeField] private float speed;

    // Start is called before the first frame update
    void Start()
    {
        platform = GetComponentInChildren<Rigidbody2D>();
        pos1 = transform.Find("Pos1");
        pos2 = transform.Find("Pos2");
        currTarget = pos1;
    }

    // Update is called once per frame
    void Update()
    {
        platform.transform.position = Vector2.MoveTowards(platform.transform.position, currTarget.position, speed * Time.deltaTime);

        if (Vector2.Distance(pos1.position, platform.transform.position) <= 0.1) {
            currTarget = pos2;
        }
        else if(Vector2.Distance(pos2.position, platform.transform.position) <= 0.1) {
            currTarget = pos1;
        }
    }
}
