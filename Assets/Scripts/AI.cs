using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float jumpForce = 2f;
    public LayerMask ground;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isJumping;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }
}
