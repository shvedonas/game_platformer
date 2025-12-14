using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatforms : MonoBehaviour
{
    public float WaitTime = 2.0f;
    public float FallTime = 1.0f;
    public float ResetTime = 1.0f;
    private bool isFalling;
    private Rigidbody2D rb;
    private Collider2D col;
    private Vector3 position;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        position = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")&&!isFalling)
        {
            StartCoroutine(Fall());
        }
    }

    private IEnumerator Fall()
    {
        isFalling = true;
        yield return new WaitForSeconds(WaitTime);
        rb.bodyType = RigidbodyType2D.Dynamic;
        yield return new WaitForSeconds(FallTime);
        col.enabled = false;
        spriteRenderer.enabled = false;
        rb.bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(ResetTime);
        col.enabled = true;
        spriteRenderer.enabled=true;
        transform.position = position;
        isFalling=false;
    }
}
