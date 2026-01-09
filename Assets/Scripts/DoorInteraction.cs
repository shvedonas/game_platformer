using System.Collections;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [Header("Настройки двери")]
    [SerializeField] private string requiredItemId = "Key_Start";
    [SerializeField] private GameObject interactButtonUI;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Collider2D physicsCollider;
    [SerializeField] private SpriteRenderer sprite;

    [Header("Фея")]
    [SerializeField] private FairyController fairyController; 
    private bool fairyEventWasTriggered = false;

    private bool isPlayerInRange;
    private bool isDoorOpen = false;
    private UniqueObject uniqueObj;

    private void Awake()
    {
        uniqueObj = GetComponent<UniqueObject>();
    }

    private void Start()
    {
        if (interactButtonUI) interactButtonUI.SetActive(false);
        if (!GameSession.IsNewGame && DatabaseManager.instance.IsObjectDestroyed(uniqueObj.uniqueId))
        {
            SetOpenStateImmediate();
        }
    }

    private void Update()
    {
        if (isPlayerInRange && !isDoorOpen && Input.GetKeyDown(KeyCode.E))
        {
            InventoryManager.Instance.OpenForSelection(CheckItem);
        }
    }

    private void CheckItem(string selectedItemId)
    {
        if (selectedItemId == requiredItemId)
        {
            InventoryManager.Instance.RemoveItem(selectedItemId);
            StartCoroutine(OpenDoorSequence());
        }
        else
        {
            if (fairyController != null && !fairyEventWasTriggered)
            {
                fairyEventWasTriggered = true; 
                InventoryManager.Instance.CloseInventory();
                fairyController.StartFairySequence();
            }
            else
            {
                NotificationUI.Instance.ShowNotification("Не к чему применить!", 2f);
            }
        }
    }

    private IEnumerator OpenDoorSequence()
    {
        InventoryManager.Instance.CloseInventory();
        Entity player = SwitchCharacter.ActiveCharacter.GetComponent<Entity>();
        if (player != null) player.LockInput(true);
        DatabaseManager.instance.MarkAsDestroyedTemporary(uniqueObj.uniqueId);
        isDoorOpen = true;
        if (interactButtonUI) interactButtonUI.SetActive(false);
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
            yield return new WaitForSeconds(1f);
        }
        if (player != null) player.LockInput(false);
        if (physicsCollider != null) physicsCollider.enabled = false;
    }

    private void SetOpenStateImmediate()
    {
        isDoorOpen = true;
        if (physicsCollider != null) physicsCollider.enabled = false;
        if (sprite != null) sprite.enabled = false;
        if (doorAnimator != null) doorAnimator.Play("Open", 0, 1.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isDoorOpen)
        {
            isPlayerInRange = true;
            if (interactButtonUI) interactButtonUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactButtonUI) interactButtonUI.SetActive(false);
        }
    }
}