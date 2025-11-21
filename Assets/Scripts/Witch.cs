using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.U2D;

public class Witch : Entity
{
    [SerializeField] private Vector2 attackSize = new Vector2(5.0f, 1.0f);
    [SerializeField] private float attackForwardOffset = 1.0f;
    [SerializeField] private float attackUpOffset = 0.5f;
    [SerializeField] private LayerMask Enemy;

    private void Awake()
    {
        damage = 10;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        isInitiallyFlipped = sprite.flipX;
    }

    private void FixedUpdate()
    {
        checkGround();

        if (jumpRequest)
        {
            Jump();
            jumpRequest = false;
        }
    }

    private void Update()
    {
        AnimatorForCharacters();

        if (Input.GetButtonDown("Fire1"))
        {
            anim.SetTrigger("AttackWitch");
            Debug.Log("Запуск атаки через триггер");
            Damage();
        }
    }

    public override void Damage()
    {
        float direction = sprite.flipX ? -1f : 1f;

        Vector2 attackPosition = (Vector2)transform.position + new Vector2((attackForwardOffset + attackSize.x / 2f) * direction, attackUpOffset);

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPosition, attackSize, 0f, Enemy);

        foreach (Collider2D enemy in hitEnemies)
        {
            Entity target = enemy.GetComponent<Entity>();
            if (target != null)
            {
                target.TakeDamage(this);
                Debug.Log($"{name} атаковал {target.name} на {damage} урона, осталось HP {target.health}");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (sprite == null)
            sprite = GetComponentInChildren<SpriteRenderer>();

        if (sprite == null) return;

        float direction = sprite.flipX ? -1f : 1f;
        Vector2 attackPosition = (Vector2)transform.position + new Vector2(attackForwardOffset * direction, attackUpOffset);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPosition, attackSize);
    }
}