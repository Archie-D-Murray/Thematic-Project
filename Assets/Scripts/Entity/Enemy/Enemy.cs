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
        }
        public int Walk;
    }

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
    }

    protected abstract void InitAnimations();
    protected virtual void Idle() { }
    protected virtual void Patrol() { }
    protected virtual void Attack() { }
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
        _isDead = true;
        _collider.enabled = false;
        print("dead");
    }

    public override void EnterPlayMode() {
        base.EnterPlayMode();
        _isDead = false;
        _collider.enabled = true;
    }

    public override void ContinuePlayMode() {
        _collider.enabled = !_isDead;
        base.ContinuePlayMode();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.gameObject.TryGetComponent(out PlayerController player)) {
            print("player");
            if (player.IsVulnerable()) {
                player.Death();
            } else {
                OnDeath();
            }
        } else {
            print("no player :(");
        }
    }
}