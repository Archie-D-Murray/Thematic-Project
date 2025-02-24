using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorButton : MonoBehaviour
{

    Door door;
    // Start is called before the first frame update
    void Start()
    {
        door = transform.parent.GetComponent<Door>();
    }

    // Update is called once per frame
    void Update()
    {
        door.toggle();
    }
}
