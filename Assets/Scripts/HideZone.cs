using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class HideZone : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color c;
    public float transparency;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        c = sr.color;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            c.a = transparency;
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
