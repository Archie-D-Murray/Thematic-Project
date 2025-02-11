using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingyLazor : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = -360.0f;
    BoxCollider2D laserCollider;
    // Start is called before the first frame update
    void Start()
    {
        laserCollider = GetComponentInChildren<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.AngleAxis(transform.rotation.eulerAngles.z+rotationSpeed*Time.deltaTime, Vector3.forward);
    }

/*    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("collision");
    }*/
}
