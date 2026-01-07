using System.Xml;
using UnityEngine;
public class PickableItem : MonoBehaviour
{
    [Header("Настройки предмета")]
    public string itemId; 

    private bool isPlayerInRange = false;
    [SerializeField] private GameObject e;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            e.SetActive(true);
            isPlayerInRange = true;
            Debug.Log($"Можно подобрать: {itemId} (Нажми E)");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            e.SetActive(false);
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
        if (InventoryManager.Instance != null)
        {
            bool added = InventoryManager.Instance.AddItem(itemId);

            if (added)
            {
                UniqueObject unique = GetComponent<UniqueObject>();
                if (unique != null)
                {
                    
                    DatabaseManager.instance.MarkAsDestroyedTemporary(unique.uniqueId);
                }

                Debug.Log($"Предмет {itemId} подобран!");
                Destroy(gameObject);
            }
        }
    }
}