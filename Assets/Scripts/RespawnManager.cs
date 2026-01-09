using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;
    public bool forceSpawnAtStartPoint = false;
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

    public IEnumerator DieSequence(Entity entity, float waitTime = 2.5f)
    {
        forceSpawnAtStartPoint =
        SceneManager.GetActiveScene().name == "Examination";

        foreach (DoorOut door in FindObjectsOfType<DoorOut>())
        {
            door.ResetDoorIfNotSaved();
        }

        yield return new WaitForSeconds(waitTime);

        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.FadeOut();

        yield return RespawnFromSaveRoutine(entity);

        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.FadeIn();

        entity.rb.simulated = true;
        entity.isDead = false;

        forceSpawnAtStartPoint = false;
    }

    private IEnumerator RespawnFromSaveRoutine(Entity entity)
    {
        Debug.LogWarning("Очищен список.");
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
            {
                if (switcher.ForceSpawnAtStart)
                {

                    if (ent is Knight)
                    {
                        ent.health = data.KnightHealth;
                    }
                    else if (ent is Witch)
                    {
                        ent.health = data.WitchHealth;
                    }
                    else if (ent is Cat)
                    {
                        ent.health = data.CatHealth;
                    }

                    Debug.Log("RespawnManager: Позиция проигнорирована (ForceSpawnAtStart), здоровье восстановлено.");
                }
                else
                {
                    ent.LoadEntityData(data);
                }
            }

            if (CharacterUIManager.Instance != null)
                CharacterUIManager.Instance.SetCharacter(ent);
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.LoadInventoryFromDB();
        }
    }
}
