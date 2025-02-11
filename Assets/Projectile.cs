using System.Collections;
using System.Collections.Generic;

using UnityEditor.Rendering;

using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField] private float speed;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       // transform.position.y = transform.position.y+speed*Time.deltaTime;
    }
}
