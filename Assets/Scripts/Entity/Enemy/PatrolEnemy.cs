using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utilities;

public class PatrolEnemy : Enemy
{
    [SerializeField] private float speed = 2;
    [SerializeField] private float[] patrolPoints = new float[2];
    [SerializeField] private int patrolIndex;
    [SerializeField] private float patrolRadius = 2;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        patrolPoints[0] = transform.position.x - patrolRadius;
        patrolPoints[1] = transform.position.x + patrolRadius;
    }

    private void FixedUpdate() {
        UpdateAnimations();
        Patrol();
    }

    protected override void Patrol() 
    {
        Vector2 targetPosition = Vector2.MoveTowards(rb2D.position, new Vector2(patrolPoints[patrolIndex], rb2D.position.y), speed * Time.fixedDeltaTime);
        rb2D.MovePosition(targetPosition);

        if(Mathf.Abs(rb2D.position.x - patrolPoints[0]) <= 0.1f) {
            patrolIndex = 1;
        }
        else if(Mathf.Abs(rb2D.position.x - patrolPoints[1]) <= 0.1f)
        {
            patrolIndex = 0;
        }
        spriteRenderer.flipX = patrolPoints[patrolIndex] > rb2D.position.x;
    }

    private void UpdateAnimations() {
        PlayAnimation(animations.Walk);     
    }

    protected override void InitAnimations() {
        animations = new EnemyAnimations("Goblin");
    }
}
