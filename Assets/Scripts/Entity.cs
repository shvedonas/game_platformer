using InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;
public class Entity : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float speed = 5.0f;
    public float horizontalMovement;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5.0f;

    [Header("GroundCheck")]
    public Transform checkGround;
    public Vector2 groundCheckSize= new Vector2(0.5f,0.05f);
    public LayerMask groundLayer;

    [Header("WallCheck")]
    public Transform checkWall;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask wallLayer;
    public bool side = true;

    [Header("WallMovement")]
    public float wallSlideSpeed;
    public bool isWallSliding;
    public bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f,12f);

    [Header("Attack")]
    [SerializeField] public int damage = 0;

    [Header("Combat")]
    public bool isAttacking;

    public Rigidbody2D rb;
    public Animator anim;
    public ParticleSystem effect;
    public SpriteRenderer sprite;
    public bool jumpRequest;
    public bool isInitiallyFlipped;
    public bool isDead = false;
    public int maxHealth = 5;
    public int health;
    public void Move(InputAction.CallbackContext context)
    {
        if(isDead == false)
        horizontalMovement = context.ReadValue<Vector2>().x;
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (CheckGround()) {
            if (context.performed && !isDead)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                anim.SetTrigger("jump");
                effect.Play();
            }
        }
        if (context.performed && wallJumpTimer > 0f && !isDead) {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0f;
            anim.SetTrigger("jump");
            effect.Play();
            if (transform.localScale.x != wallJumpDirection) {
                side = !side;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
            }
            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
        }
    }

    public void Flip()
    {
        if (side && horizontalMovement < 0 || !side && horizontalMovement > 0) {
            side = !side;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
            if (rb.velocity.y != 0)
            {
                effect.Play();
            }
        }
    }
    public bool CheckGround()
    {
        if (Physics2D.OverlapBox(checkGround.position,groundCheckSize,0,groundLayer))
        {
            return true;
        }
        return false;
    }
    
    public bool CheckWall()
    {
        if (Physics2D.OverlapBox(checkWall.position, wallCheckSize, 0, wallLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SlowSlide()
    {
        if (!CheckGround() && CheckWall() && horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2 (rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideSpeed));   
        }
        else
        {
            isWallSliding = false;
        }
    }

    public void CancelWallJump()
    {
        isWallJumping = false;
    }

    public void JumpSlide()
    {
        if (isWallSliding) {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;
            CancelInvoke(nameof(CancelWallJump));
        }
        else if(wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }

    }

    public virtual void PerformAttack()
    {
    }

    public void FinishAttack()
    {
        isAttacking = false;
    }
    
    public virtual void Die()
    {
        if (isDead) return;

        isDead = true;
        rb.simulated = false; 
        anim.SetTrigger("Dead");

        StartCoroutine(RespawnManager.Instance.DieSequence(this, 1f));
    }


    public virtual void TakeDamage(Entity attacker)
    {
        if (isDead) return;

        health -= attacker.damage;

        if (CharacterUIManager.Instance != null &&
            SwitchCharacter.ActiveCharacter == gameObject)
        {
            CharacterUIManager.Instance.OnHealthChanged(health);
        }

        if (health <= 0)
            Die();
    }

    public void SaveEntityData(Vector3? overridePosition = null)
    {
        Vector3 positionToSave = overridePosition ?? transform.position;

        string myType = this.GetType().Name;
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        SaveData data = new SaveData(
            GameSession.CurrentSlotIndex,
            currentScene,
            myType,
            positionToSave,
            health, 
            damage
        );

        DatabaseManager.instance.SaveGame(data);
        Debug.Log("Игра сохранена на чекпоинте!");
    }

    public void LoadEntityData(SaveData data)
    {
        transform.position = new Vector3(data.PositionX, data.PositionY, data.PositionZ);
        health = data.Health;
        damage = data.Damage;
        Debug.Log("Персонаж загружен из базы!");
    }
    public virtual void Damage()
    {
    }



}