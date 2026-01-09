using UnityEngine;

public class Spikes : MonoBehaviour
{
    [Header("Настройки")]
    public float bounceForce = 15f;    
    public float knockbackForce = 10f; 
    public int damage = 1;
    private Entity dummyAttacker;
    private bool knight;
    private void Start()
    {
        dummyAttacker = gameObject.AddComponent<Entity>();
        dummyAttacker.damage = damage;
        dummyAttacker.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Entity playerEntity = collision.gameObject.GetComponent<Entity>();
            if (playerEntity != null && playerEntity.health > 1)
            {
                if(playerEntity.gameObject.name == "Knight")
                {
                    knight = true;
                }
                else
                {
                    knight = false;
                }
                    HandlePlayerBounce(collision);
            }
                if (damage > 0)
                {
                    if (playerEntity != null && !playerEntity.isDead)
                    {
                        dummyAttacker.damage = damage;
                        playerEntity.TakeDamage(dummyAttacker);
                    }
                }         
        }
    }

    private void HandlePlayerBounce(Collision2D collision)
    {
        if (knight) bounceForce = 800;
        else bounceForce = 700;
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.velocity = Vector2.zero;
            bool isTopCollision = collision.transform.position.y > transform.position.y + 0.3f;

            if (isTopCollision)
            {
                rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
            }
            else
            {
                float direction = Mathf.Sign(collision.transform.position.x - transform.position.x);
                Vector2 knockback = new Vector2(direction * knockbackForce, bounceForce * 0.5f);

                rb.AddForce(knockback, ForceMode2D.Impulse);
            }
        }
    }
}