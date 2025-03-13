using LevelEditor;

using Tags.Obstacle;

using UnityEngine;

public class PatrolEnemy : Enemy {
    [SerializeField] private float speed = 2;
    [SerializeField] private Transform[] patrolPoints = new Transform[2];
    [SerializeField] private int patrolIndex;

    protected override Transform[] GetMoveables() {
        if (!patrolPoints[0] || !patrolPoints[1]) {
            GetPositions();
        }
        return new Transform[] { transform };
    }

    private void GetPositions() {
        foreach (Transform child in transform) {
            if (child.gameObject.GetComponent<FirstPosition>()) {
                patrolPoints[0] = child;
            } else if (child.gameObject.GetComponent<SecondPosition>()) {
                patrolPoints[1] = child;
            }
        }
    }

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
    }

    private void FixedUpdate() {
        UpdateAnimations();
        Patrol();
    }

    protected override void Patrol() {
        Vector2 targetPosition = Vector2.MoveTowards(rb2D.position, patrolPoints[patrolIndex].position, speed * Time.fixedDeltaTime);
        rb2D.MovePosition(targetPosition);

        if (Mathf.Abs(rb2D.position.x - patrolPoints[patrolIndex].position.x) <= 0.1f) {
            patrolIndex = ++patrolIndex % patrolPoints.Length;
        }
        spriteRenderer.flipX = patrolPoints[patrolIndex].position.x > rb2D.position.x;
    }

    private void UpdateAnimations() {
        PlayAnimation(animations.Walk);
    }

    protected override void InitAnimations() {
        animations = new EnemyAnimations("Goblin");
    }
}