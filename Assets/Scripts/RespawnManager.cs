using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator DieSequence(Entity entity, float waitTime = 1f)
    {
        yield return new WaitForSeconds(waitTime);
        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeOut();
        }

        yield return RespawnFromSaveRoutine(entity);

        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.FadeIn();

        entity.rb.simulated = true;
        entity.isDead = false;
    }

    private IEnumerator RespawnFromSaveRoutine(Entity entity)
    {
        SaveData data = DatabaseManager.instance.LoadGame(GameSession.CurrentSlotIndex);
        if (data == null)
        {
            Debug.LogWarning("Сейв не найден! Респаун невозможен.");
            yield break;
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(data.SceneName);
        while (!asyncLoad.isDone)
            yield return null;

        yield return null;
        SwitchCharacter switcher = FindObjectOfType<SwitchCharacter>();
        if (switcher != null)
        {
            GameObject charObj = switcher.GetCharacterObjectByType(data.PlayerType);
            switcher.ActivateCharacter(charObj);

            Entity ent = charObj.GetComponent<Entity>();
            if (ent != null)
                ent.LoadEntityData(data);
            if (CharacterUIManager.Instance != null)
                CharacterUIManager.Instance.SetCharacter(ent);
        }
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.LoadInventoryFromDB();
        }

    }
}
