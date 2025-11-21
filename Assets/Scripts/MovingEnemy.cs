using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class movingmosner : Entity
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private Vector2 attackSize = new Vector2(1.5f, 1.0f);
    [SerializeField] private float attackForwardOffset = 1.0f;
    [SerializeField] private float attackUpOffset = 0.5f;
    [SerializeField] private LayerMask Enemy;
    [SerializeField] private float attackCooldown = 2.0f;
    [SerializeField] private float attackRange = 2.0f;

    private SpriteRenderer sprite;
    private bool movingToB = true;
    private float lastAttackTime;
    private bool isAttacking = false;

    private void Awake()
    {
        health = 50;
        anim = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        lastAttackTime = -attackCooldown; 
    }

    private void Update()
    {
        if (isDead){
            return;
        }
        if (!isAttacking)
        {
            Patrol();
        }

        CheckForAttack();
    }

    private void CheckForAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        float direction = sprite.flipX ? -1f : 1f;
        Vector2 attackPosition = (Vector2)transform.position + new Vector2(attackForwardOffset * direction, attackUpOffset);

        Collider2D targetCollider = Physics2D.OverlapBox(attackPosition, attackSize, 0f, Enemy);

        if (targetCollider != null)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        state = States.jump;
        anim.SetTrigger("AttackSceleton");
        lastAttackTime = Time.time;

        StopAllCoroutines();
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(0.3f);

        ApplyDamage();

        yield return new WaitForSeconds(0.7f);

        isAttacking = false;
    }

    private void ApplyDamage()
    {
        float direction = sprite.flipX ? -1f : 1f;
        Vector2 attackPosition = (Vector2)transform.position + new Vector2(attackForwardOffset * direction, attackUpOffset);

        Collider2D targetCollider = Physics2D.OverlapBox(attackPosition, attackSize, 0f, Enemy);
        if (targetCollider != null)
        {
            Entity target = targetCollider.GetComponent<Entity>();
            if (target != null)
            {
                target.TakeDamage(this);
            }
        }
    }

    private void Patrol()
    {
        state = States.walk;

        Vector3 destination = movingToB ? pointB.position : pointA.position;
        Vector3 direction = destination - transform.position;

        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        if (sprite != null)
            sprite.flipX = direction.x < 0f;

        if (direction.magnitude < 0.5f)
        {
            movingToB = !movingToB;
        }
    }

    public override void TakeDamage(Entity attacker)
    {
        if (isDead) { 
            return;
        }
        health -= attacker.damage;
        Debug.Log($"{name} получил {attacker.damage} урона, осталось HP {health}");

        if (health <= 0) { 

            StartDeath(); 
        }
    }
    private void StartDeath()
    {
        isDead = true;

        StopAllCoroutines();

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        anim.SetTrigger("Die"); 

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        yield return new WaitForSeconds(1.5f);

        anim.enabled = false;

    }



    private void OnDrawGizmosSelected()
    {
        if (sprite != null)
        {
            float direction = sprite.flipX ? -1f : 1f;
            Vector2 attackPosition = (Vector2)transform.position + new Vector2(attackForwardOffset * direction, attackUpOffset);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(attackPosition, attackSize);
        }
    }
}
