using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class HideZone : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color c;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        c = sr.color;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            c.a = 0.5f;
            sr.color = c;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            c.a = 1f;
            sr.color = c;
        }
    }
}
