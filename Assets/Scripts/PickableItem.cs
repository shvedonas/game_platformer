using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [Header("Настройки предмета")]
    public string itemId; 

    private bool isPlayerInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log($"Можно подобрать: {itemId} (Нажми E)");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }

    private void PickUp()
    {
        if (InventoryManager.instance != null)
        {
            bool added = InventoryManager.instance.AddItem(itemId);

            if (added)
            {
                UniqueObject unique = GetComponent<UniqueObject>();
                if (unique != null)
                {
                    DatabaseManager.instance.AddDestroyedObject(unique.uniqueId);
                }

                Debug.Log($"Предмет {itemId} подобран!");
                Destroy(gameObject);
            }
        }
    }
}