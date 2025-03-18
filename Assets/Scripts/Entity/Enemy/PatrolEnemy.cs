using Data;

using LevelEditor;

using Tags.Obstacle;

using UnityEngine;

public class PatrolEnemy : Enemy {
    [SerializeField] private float speed = 2;
    [SerializeField] private Transform[] patrolPoints = new Transform[2];
    [SerializeField] private int patrolIndex;
    private SpriteRenderer[] _pointRenderers;

    protected override Transform[] GetMoveables() {
        if (!patrolPoints[0] || !patrolPoints[1]) {
            GetPositions();
        }
        if (_pointRenderers == null || _pointRenderers.Length < 1) {
            _pointRenderers = new SpriteRenderer[2] { patrolPoints[0].GetComponent<SpriteRenderer>(), patrolPoints[1].GetComponent<SpriteRenderer>() };
        }
        return new Transform[] { transform, patrolPoints[0], patrolPoints[1] };
    }

    private void GetPositions() {
        foreach (Transform child in transform.parent) {
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
        OnPlaceStart += PlacementStart;
        OnPlaceFinish += PlacementFinish;
    }

    private void FixedUpdate() {
        UpdateAnimations();
        Patrol();
    }

    protected override void Patrol() {
        if (_placing) { return; }
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

    public void LoadPatrolEnemyData(PatrolEnemyData data) {
        transform.position = data.CurrentPosition;
        patrolPoints[0].position = data.Patrol1;
        patrolPoints[1].position = data.Patrol2;
    }

    public PatrolEnemyData ToPatrolEnemyData() {
        return new PatrolEnemyData(transform.position, patrolPoints[0].position, patrolPoints[1].position);
    }
    private void PlacementStart() {
        foreach (SpriteRenderer renderer in _pointRenderers) {
            renderer.Fade(Color.clear, Color.white, 0.5f, this);
        }
    }

    private void PlacementFinish() {
        rb2D.MovePosition(patrolPoints[0].position);
        foreach (SpriteRenderer renderer in _pointRenderers) {
            renderer.Fade(Color.white, Color.clear, 0.5f, this);
        }
    }
}