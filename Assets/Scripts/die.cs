using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class die : Entity
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Entity playerEntity = collision.GetComponent<Entity>();
            if (playerEntity != null)
            {
                playerEntity.Die();
            }
        }
    }
}
