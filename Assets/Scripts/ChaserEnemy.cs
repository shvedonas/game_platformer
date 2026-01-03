using UnityEngine;
using Pathfinding;
using System.Collections;


public class ChaserEnemy : Entity
{
    [Header("Combat Settings")]
    [SerializeField] private Vector2 attackSize = new Vector2(1.5f, 1.0f);
    [SerializeField] private float attackForwardOffset = 1.0f;
    [SerializeField] private float attackUpOffset = 0.5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float attackCooldown = 2.0f;
    [SerializeField] private GameObject go;

    private AIPath aiPath;
    public AIDestinationSetter ai;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D mainCollider;
    private float lastAttackTime;
    private bool isAttacking;
    private bool isHurting;

    private void Awake()
    {
        health = 50;
        damage = 1;
        ai = GetComponent<AIDestinationSetter>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        aiPath = GetComponent<AIPath>();
        mainCollider = GetComponent<BoxCollider2D>();
        lastAttackTime = -attackCooldown;
    }

    private void Update()
    {
        if (SwitchCharacter.ActiveCharacter != null) {
            SetTarget(SwitchCharacter.ActiveCharacter.transform);
        }
        if (isDead || isHurting) return;
        if (isAttacking) return;

        HandleMovement();
        CheckForAttack();
    }
    public void SetTarget(Transform target)
    {
        if (ai != null) ai.target = target;
    }
    private void HandleMovement()
    {
        if (aiPath == null) return;

        if (aiPath.desiredVelocity.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (aiPath.desiredVelocity.x < -0.01f)
            spriteRenderer.flipX = true;

        anim.SetFloat("speed", aiPath.velocity.magnitude);
    }

    private void CheckForAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        float dir = spriteRenderer.flipX ? -1f : 1f;
        Vector2 pos = (Vector2)transform.position +
                      new Vector2(attackForwardOffset * dir, attackUpOffset);

        Collider2D hit = Physics2D.OverlapBox(pos, attackSize, 0f, playerLayer);

        if (hit != null)
            StartAttack();
    }

    private void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        if (aiPath != null)
            aiPath.canMove = false;

        anim.SetTrigger("attack");
    }

    public void AE_ApplyDamage()
    {
        if (isDead || isHurting) return;

        float dir = spriteRenderer.flipX ? -1f : 1f;
        Vector2 pos = (Vector2)transform.position +
                      new Vector2(attackForwardOffset * dir, attackUpOffset);

        Collider2D hit = Physics2D.OverlapBox(pos, attackSize, 0f, playerLayer);
        if (hit == null) return;

        Entity target = hit.GetComponentInParent<Entity>();
        if (target == null) return;

        target.TakeDamage(this);
    }

    public void AE_EndAttack()
    {
        if (isHurting || isDead) return;

        isAttacking = false;

        if (aiPath != null)
            aiPath.canMove = true;
    }

    public override void TakeDamage(Entity attacker)
    {
        if (isDead) return;

        if (isAttacking)
        {
            isAttacking = false;
            anim.ResetTrigger("attack");
        }

        health -= attacker.damage;

        if (health <= 0)
        {
            StartDeath();
            return;
        }

        if (isHurting)
        {
            StopCoroutine("HurtRoutine");
        }
        StartCoroutine("HurtRoutine");
    }

    private IEnumerator HurtRoutine()
    {
        isHurting = true;

        if (aiPath != null) aiPath.canMove = false;
        anim.SetTrigger("hurt");
        yield return new WaitForSeconds(0.4f);

        isHurting = false;
        if (aiPath != null && !isDead && !isAttacking)
            aiPath.canMove = true;
    }
 
    private void StartDeath()
    {
        if (isDead) return;
        isDead = true;

        isAttacking = false;
        isHurting = false;

        if (mainCollider != null)
        {
            mainCollider.offset = new Vector2(mainCollider.offset.x, 0.2734451f);
            mainCollider.size = new Vector2(mainCollider.size.x, 0.6291656f);
        }

        if (aiPath != null)
        {
            aiPath.canMove = false;
            aiPath.enabled = false;
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = false;
            rb.gravityScale = 3f;
        }

        StopAllCoroutines();
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        anim.SetTrigger("die");

        yield return new WaitForSeconds(1.2f);

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            rb.simulated = false;
        }

        
    }

    private void OnDrawGizmosSelected()
    {
        float dir = transform.localScale.x >= 0 ? 1f : -1f;
        Vector2 pos = (Vector2)transform.position + new Vector2(attackForwardOffset * dir, attackUpOffset);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(pos, attackSize);
    }
}