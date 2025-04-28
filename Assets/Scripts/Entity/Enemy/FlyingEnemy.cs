using System.Collections;
using System.Collections.Generic;

using Data;

using Entity.Player;

using UnityEngine;

public class FlyingEnemy : Enemy {
    [SerializeField] private float speed = 2;

    protected override Transform[] GetMoveables() {
        if (!animator) {
            InitAnimations();
            animator = GetComponent<Animator>();
            rb2D = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
            player = FindFirstObjectByType<PlayerController>().transform;
        }
        return new Transform[] { transform };
    }

    protected override void Attack() {
        if (!_playing || _isDead) { return; }
        spriteRenderer.flipX = player.position.x > rb2D.position.x;
        Vector2 targetPosition = Vector2.MoveTowards(rb2D.position, player.position, speed * Time.fixedDeltaTime);
        rb2D.MovePosition(targetPosition);
    }

    protected override void EnterAttack() {
        PlayAnimation(animations.Walk);
    }

    protected override void Idle() {
        SwitchState(EnemyState.Attack);
    }

    protected override void InitAnimations() {
        animations = new EnemyAnimations("Skull");
    }

    public void LoadSaveData(StaticEnemyData data) {
        transform.position = data.CurrentPosition;
    }

    public StaticEnemyData ToSaveData() {
        return new StaticEnemyData(_initialPosition);
    }
}