using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToTheStage : MonoBehaviour
{
    [Header("Куда возвращаемся")]
    [SerializeField] private string targetSceneName = "PlayScene";
    [SerializeField] private Vector3 returnPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (SwitchCharacter.ActiveCharacter != null)
            {
                Entity playerEntity = SwitchCharacter.ActiveCharacter.GetComponent<Entity>();
                if (playerEntity != null)
                {
                    playerEntity.SaveEntityData(returnPosition, targetSceneName);
                }
            }

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.SaveInventoryToDB();
            }

            if (DatabaseManager.instance != null)
            {
                DatabaseManager.instance.CommitDestroyedObjects();
            }

            SceneManager.LoadScene(targetSceneName);
        }
    }
}
