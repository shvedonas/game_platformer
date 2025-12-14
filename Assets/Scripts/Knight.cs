using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.U2D;

public class  Knight: Entity
{
    [Header("Knight Combat Settings")]
    [SerializeField] private Vector2 attackSize = new Vector2(1.5f, 1.0f);
    [SerializeField] private float attackForwardOffset = 1.0f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float attackCooldown = 50.0f;

    [Header("Combat Settings")]
    private float lastAttackTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        isInitiallyFlipped = sprite.flipX;
    }


    private void Update()
    {
        SlowSlide();
        JumpSlide();
        if (!isWallJumping)
        {
            rb.velocity = new Vector2(speed * horizontalMovement, rb.velocity.y);
            Flip();
        }
        anim.SetFloat("yVelocity",rb.velocity.y);
        anim.SetFloat("magnitude", Mathf.Abs(rb.velocity.x));
        anim.SetBool("isWallSliding",isWallSliding);
        anim.SetBool("isGrounded", CheckGround());
    }
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && Time.time > lastAttackTime + attackCooldown && !isAttacking)
        {
            isAttacking = true;
            lastAttackTime = Time.time;
            anim.SetTrigger("attack");
        }
    }

    public override void PerformAttack()
    {
        float direction = transform.localScale.x > 0 ? 1f : -1f;
        Vector2 attackPos = (Vector2)transform.position + new Vector2(attackForwardOffset * direction, 0.5f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(attackPos, attackSize, 0f, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            Entity target = hit.GetComponent<Entity>();
            if (target != null)
            {
                target.TakeDamage(this); 
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        float direction = transform.localScale.x > 0 ? 1f : -1f;
        Vector2 attackPos = (Vector2)transform.position + new Vector2(attackForwardOffset * direction, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos, attackSize);
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(checkGround.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(checkWall.position, wallCheckSize);
    }
}