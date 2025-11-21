using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] private float attackRange = 1.0f;
    [SerializeField] private LayerMask playerLayer;

    public override void Damage()
    {
        Collider2D[] players = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer);
        foreach (Collider2D player in players)
        {
            Entity target = player.GetComponent<Entity>();
            if (target != null)
            {
                target.TakeDamage(this);
                Debug.Log($"{name} атаковал {target.name} на {damage} урона, осталось HP {target.health}");
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
