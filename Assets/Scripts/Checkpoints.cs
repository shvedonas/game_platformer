using UnityEngine;

public class Checkpoint : Entity
{
    [Header("Настройки")]
    [Tooltip("Точка, где появится игрок при загрузке. Если пусто - берется позиция этого объекта")]
    public Transform spawnPoint;

    [Header("Визуал")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public Sprite inactiveSprite; 

    private bool isActivated = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && inactiveSprite != null)
            spriteRenderer.sprite = inactiveSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Что-то вошло в триггер: {collision.gameObject.name} (Тег: {collision.tag})");
        if (collision.CompareTag("Player"))
        {
            Entity player = collision.GetComponent<Entity>();

            if (player != null)
            {
                ActivateCheckpoint(player);
            }
        }
    }

    private void ActivateCheckpoint(Entity player)
    {
        Vector3 savePos = transform.position;
        if (!isActivated)
        {
            isActivated = true;
            savePos = transform.position;
            if (spriteRenderer != null)
            {
                anim.SetTrigger("Check");
            }
            Debug.Log("Чекпоинт активирован!");
        }

        if (spawnPoint != null&&!isActivated)
        {
            savePos = spawnPoint.position;
        }
        player.SaveEntityData(savePos);
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.SaveInventoryToDB();
        }
        DatabaseManager.instance.CommitDestroyedObjects();
    }
}