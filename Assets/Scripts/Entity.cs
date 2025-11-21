using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] public float speed = 5.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] public int health = 100;
    [SerializeField] public int damage = 0;

    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer sprite;
    public bool isGround = false;
    public bool jumpRequest;
    public bool isInitiallyFlipped;
    public bool isDead = false;
    public void Walk()
    {
        Vector3 dir = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
        if (isInitiallyFlipped)
        {
            sprite.flipX = dir.x > 0.0f;
        }
        else
        {
            sprite.flipX = dir.x < 0.0f;
        }
    }

    public void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void checkGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        isGround = collider.Length > 1;
    }


    public enum States
    {
        idle,
        walk,
        jump,
        die
    }

    public States state
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    public virtual void Die()
    {
        Debug.Log($"{name} погиб!");
        Destroy(gameObject);
    }

    public void AnimatorForCharacters()
    {
        if (isGround && Input.GetButtonDown("Jump"))
        {
            jumpRequest = true;
        }

        if (Input.GetButton("Horizontal"))
        {
            Walk();
        }

        if (!isGround)
        {
            state = States.jump;
        }
        else if (Input.GetButton("Horizontal"))
        {
            state = States.walk;
        }
        else
        {
            state = States.idle;
        }
    }

    public virtual void TakeDamage(Entity attacer)
    {

        health -= attacer.damage;
        Debug.Log($"{name} получил {damage} урона, осталось HP {health}");

        if (health <= 0)
            Die();
    }

    public virtual void Damage()
    {
    }
}