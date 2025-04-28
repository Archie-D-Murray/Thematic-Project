using System.Collections;
using System.Collections.Generic;

using Data;

using Entity.Player;

using UnityEngine;

using Utilities;

using static Enemy;
using static UnityEngine.ParticleSystem;

public class TurretEnemy : Enemy {
    [SerializeField] private GameObject enemyProjectile;
    [SerializeField] private float projectileSpeed = 5;
    private CountDownTimer attackTimer = new CountDownTimer(0f);
    [SerializeField] private float attackTime;
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

    protected override void Start() {
        base.Start();
        PlayAnimation(animations.Walk);
        attackTimer.Start();
    }
    protected override void Attack() {
        if (!_playing || _isDead) { return; }
        attackTimer.Update(Time.fixedDeltaTime);
        if (!IsInRange()) {
            SwitchState(EnemyState.Idle);
            return;
        }
        spriteRenderer.flipX = player.position.x > rb2D.position.x;
        Vector2 shootDirection = ((Vector2)player.position - (Vector2)this.transform.position).normalized;
        Quaternion rotation = Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up, shootDirection), Vector3.forward);

        if (attackTimer.IsFinished) {
            GameObject projectile = Instantiate(enemyProjectile, this.transform.position, rotation);
            projectile.AddComponent<ProjectileMover>().Move(projectileSpeed, 3f, shootDirection);
            attackTimer.Reset(attackTime);
        }
    }

    protected override void EnterAttack() {
        PlayAnimation(animations.Walk);
    }

    protected override void Idle() {
        if (IsInRange())
            SwitchState(EnemyState.Attack);
    }

    protected override void InitAnimations() {
        animations = new EnemyAnimations("Turret");
    }

    public void LoadSaveData(StaticEnemyData data) {
        transform.position = data.CurrentPosition;
    }

    public StaticEnemyData ToSaveData() {
        return new StaticEnemyData(_initialPosition);
    }
}