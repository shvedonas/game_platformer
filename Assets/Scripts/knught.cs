using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class knught : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private int health = 5;
    [SerializeField] private float jumpForce = 5.0f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private bool isGround = false;
    private bool jumpRequest;
    private bool isInitiallyFlipped;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        isInitiallyFlipped = sprite.flipX;
    }

    private void Walk()
    {
        // Убрал установку состояния отсюда - это вызывает конфликт
        Vector3 dir = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
        if (isInitiallyFlipped)
        {
            sprite.flipX = dir.x > 0.0f; // инвертировали для изначально повернутого кота
        }
        else
        {
            sprite.flipX = dir.x < 0.0f; // стандартная логика
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void checkGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        isGround = collider.Length > 1;
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
        // Запрос на прыжок
        if (isGround && Input.GetButtonDown("Jump"))
        {
            jumpRequest = true;
        }

        // Движение по горизонтали
        if (Input.GetButton("Horizontal"))
        {
            Walk();
        }

        // Управление анимациями - ПРАВИЛЬНЫЙ ПОРЯДОК
        if (!isGround)
        {
            state = States.jump; // прыжок имеет наивысший приоритет
        }
        else if (Input.GetButton("Horizontal"))
        {
            state = States.walk; // ходьба
        }
        else
        {
            state = States.idle; // покой
        }
    }

    public enum States
    {
        idle,
        walk,
        jump
    }

    private States state
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }
}