using UnityEngine;

public class NPC_RiddleGiver : MonoBehaviour
{
    [SerializeField] private RiddleMaster riddleMaster; 
    [SerializeField] private GameObject promptUI; 

    private bool isPlayerInRange;

    private void Start()
    {
        if (promptUI) promptUI.SetActive(false);
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            riddleMaster.StartInteraction();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (promptUI) promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (promptUI) promptUI.SetActive(false);
        }
    }
}