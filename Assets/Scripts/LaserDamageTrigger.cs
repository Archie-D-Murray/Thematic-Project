using System.Collections;
using System.Collections.Generic;

using Entity.Player;

using LevelEditor;

using UnityEngine;

public class LaserDamageTrigger : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.HasComponent<PlayerController>()) {
            Destroy(collision.gameObject);
        }
    }
}