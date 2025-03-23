using System.Collections;
using System.Collections.Generic;

using Data;

using UnityEngine;

public class FlyingEnemy : Enemy {
    [SerializeField] private float speed = 2;

    protected override Transform[] GetMoveables() {
        return new Transform[] { transform };
    }

    // Update is called once per frame
    private void FixedUpdate() {
        UpdateAnimations();
        AttackPlayer();
    }

    private void AttackPlayer() {
        if (IsInRange()) {
            Vector2 targetPosition = Vector2.MoveTowards(rb2D.position, player.position, speed * Time.fixedDeltaTime);
            rb2D.MovePosition(targetPosition);
        }
    }
    private void UpdateAnimations() {
        PlayAnimation(animations.Walk);
        spriteRenderer.flipX = player.position.x > rb2D.position.x;
    }

    protected override void InitAnimations() {
        animations = new EnemyAnimations("Skull");
    }

    public void LoadSaveData(FlyingEnemyData data) {
        transform.position = data.CurrentPosition;
    }

    public FlyingEnemyData ToSaveData() {
        return new FlyingEnemyData(transform.position);
    }
}