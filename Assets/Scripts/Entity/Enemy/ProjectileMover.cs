using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMover : MonoBehaviour
{
    private Rigidbody2D rigidbodyProjectile;
    private float duration;
    // Start is called before the first frame update
    private void Awake() {
        rigidbodyProjectile = GetComponent<Rigidbody2D>();
    }

    public void Move(float speed, float duration, Vector2 shootDirection) {
        this.duration = duration;
        rigidbodyProjectile.velocity = new Vector2(shootDirection.x * speed, shootDirection.y * speed);
        Destroy(gameObject, duration);
    }
}
