using System.Collections;
using System.Collections.Generic;

using Entity.Player;

using LevelEditor;

using UnityEngine;
using UnityEngine.Windows;

using Utilities;

public abstract class Enemy : Placeable {
    protected class EnemyAnimations {
        public EnemyAnimations(string name) {
            Walk = Animator.StringToHash($"{name}_Walk");
            Death = Animator.StringToHash("Enemy_Death");
        }
        public int Walk;
        public int Death;
    }
    public enum EnemyState { Idle, Patrol, Attack, Death }
    [SerializeField] protected EnemyState state;
    [SerializeField] protected Transform player;
    [SerializeField] protected Animator animator;
    [SerializeField] protected int currentAnimation = 0;
    [SerializeField] protected float attackRange = 2;

    [SerializeField] protected Rigidbody2D rb2D;
    [SerializeField] protected EnemyAnimations animations;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected BoxCollider2D _collider;

    protected bool _isDead;

    protected virtual void Start() {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();

        player = FindFirstObjectByType<PlayerController>().transform;

        InitAnimations();
        InitReferences();
        SwitchState(InitialState());
    }

    private void FixedUpdate() {
        if (state == EnemyState.Death) {
            return;
        }
        if (!player) {
            state = EnemyState.Idle;
        }

        switch (state) {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            default:
                break;
        }
    }
    protected virtual EnemyState InitialState() => EnemyState.Idle;

    protected void SwitchState(EnemyState state) {
        switch (state) {
            case EnemyState.Idle:
                EnterIdle();
                break;
            case EnemyState.Patrol:
                EnterPatrol();
                break;
            case EnemyState.Attack:
                EnterAttack();
                break;
            default:
                break;
        }
        this.state = state;
    }
    protected abstract void InitAnimations();
    protected virtual void Idle() { }
    protected virtual void Patrol() { }
    protected virtual void Attack() { }
    protected virtual void EnterIdle() { }
    protected virtual void EnterPatrol() { }
    protected virtual void EnterAttack() { }
    protected virtual bool IsInRange() {
        if (!player) {
            return false;
        }
        return Vector2.Distance(transform.position, player.position) <= attackRange;
    }

    protected void PlayAnimation(int animation) {
        if (currentAnimation == animation) { return; }
        currentAnimation = animation;
        animator.Play(animation);
    }

    protected virtual void OnDeath() {
        PlayAnimation(animations.Death);
        _isDead = true;
        _collider.enabled = false;
        print("dead");
    }

    public override void EnterPlayMode() {
        base.EnterPlayMode();
        _isDead = false;
        _collider.enabled = true;
        PlayAnimation(animations.Walk);
    }

    public override void ContinuePlayMode() {
        _collider.enabled = !_isDead;
        base.ContinuePlayMode();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.TryGetComponent(out PlayerController player)) {
            print("player");
            if (player.IsVulnerable()) {
                player.Death();
            } else {
                player.OnKill?.Invoke();
                OnDeath();
            }
        } else {
            print("no player :(");
        }
    }
}