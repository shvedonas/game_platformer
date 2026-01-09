using Cinemachine;
using System.Collections;
using UnityEngine;

public class SwitchCharacter : MonoBehaviour
{
    public static SwitchCharacter Instance;
    public bool ForceSpawnAtStart = false;
    public static GameObject ActiveCharacter { get; private set; }

    [Header("Персонажи (ссылки на объекты сцены)")]
    [SerializeField] public GameObject knight;
    [SerializeField] public GameObject witch;
    [SerializeField] public GameObject cat;

    [Header("Настройки")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    public bool catActive = false;
    public bool witchActive = false;
    private GameObject currentCharacter;
    private FairyHintTrigger trigger;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start()
    {
        Time.timeScale = 1.0f;
        if (knight) knight.SetActive(false);
        if (witch) witch.SetActive(false);
        if (cat) cat.SetActive(false);

        yield return null;

        if (DatabaseManager.instance == null)
        {
            Debug.LogWarning("DatabaseManager не найден! Запускаем рыцаря по умолчанию.");
            ActivateCharacter(knight);
            yield break;
        }

        if (GameSession.IsNewGame)
        {
            Debug.Log("Новая игра: Сброс позиций и здоровья.");

            if (startPoint != null)
            {
                knight.transform.position = startPoint.position;
                witch.transform.position = startPoint.position;
                cat.transform.position = startPoint.position;
            }

            ResetHealth(knight);
            ResetHealth(witch);
            ResetHealth(cat);

            ActivateCharacter(knight);
            DatabaseManager.instance.ClearSaveSlot(GameSession.CurrentSlotIndex);
            Entity entity = knight.GetComponent<Entity>();
            if (entity != null) entity.SaveEntityData();

            GameSession.IsNewGame = false;
        }
        else
        {
            SaveData data = DatabaseManager.instance.LoadGame(GameSession.CurrentSlotIndex);

            if (data != null)
            {
                ApplyHealth(knight, data.KnightHealth);
                ApplyHealth(witch, data.WitchHealth);
                ApplyHealth(cat, data.CatHealth);

                GameObject targetChar = GetCharacterObjectByType(data.PlayerType);
                if (targetChar == null) targetChar = knight;

                if (ForceSpawnAtStart && startPoint != null)
                {
                    targetChar.transform.position = startPoint.position;
                }
                else
                {
                    targetChar.transform.position = new Vector3(
                       data.PositionX,
                       data.PositionY,
                       0f
                   );
                }

                ActivateCharacter(targetChar);
            }
        }

    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1)) SwitchC(knight);
        else if (Input.GetKeyUp(KeyCode.Alpha2)) SwitchC(witch);
        else if (Input.GetKeyUp(KeyCode.Alpha3)) SwitchC(cat);
    }

    private void SwitchC(GameObject targetCharacter)
    {
        if (targetCharacter == null || targetCharacter == currentCharacter) return;
        if (Input.GetKeyUp(KeyCode.Alpha3) && !catActive) return;
        if(Input.GetKeyUp(KeyCode.Alpha2) && !witchActive) return;
        Entity currentEntity = currentCharacter.GetComponent<Entity>();
        if (currentEntity != null && currentEntity.isDead) return;
        if (currentEntity != null && currentEntity.isInputLocked)
            return;
        Vector3 lastPosition = currentCharacter.transform.position;
        BoxCollider2D newCollider = targetCharacter.GetComponent<BoxCollider2D>();

        if (newCollider != null)
        {
            float targetHeight = newCollider.size.y * Mathf.Abs(targetCharacter.transform.localScale.y);

            float checkWidth = 0.1f; 
            float groundBuffer = 0.1f; 

            Vector2 checkSize = new Vector2(checkWidth, targetHeight - groundBuffer);
            Vector2 checkCenter = (Vector2)lastPosition + Vector2.up * (groundBuffer + checkSize.y * 0.5f);

            LayerMask groundMask = LayerMask.GetMask("Ground");
            Collider2D hit = Physics2D.OverlapBox(checkCenter, checkSize, 0f, groundMask);

            if (hit != null)
            {
                Debug.Log($"Нельзя переключиться на {targetCharacter.name} — мало места (потолок)!");
                return;
            }
        }

        targetCharacter.transform.position = lastPosition;

        ActivateCharacter(targetCharacter);
    }

    public void ActivateCharacter(GameObject character)
    {
        if (character == null) return;

        if (currentCharacter != null)
            currentCharacter.SetActive(false);

        currentCharacter = character;
        currentCharacter.SetActive(true);
        ActiveCharacter = currentCharacter;

        if (virtualCamera != null)
            virtualCamera.Follow = currentCharacter.transform;

        if (CharacterUIManager.Instance != null)
        {
            CharacterUIManager.Instance.UpdateFrames(currentCharacter);
        }

        Entity entity = currentCharacter.GetComponent<Entity>();
        if (entity != null && CharacterUIManager.Instance != null)
        {
            CharacterUIManager.Instance.SetCharacter(entity);
        }
    }

    public GameObject GetCharacterObjectByType(string typeName)
    {
        if (string.IsNullOrEmpty(typeName)) return null;

        string type = typeName.ToLower();
        if (type.Contains("knight")) return knight;
        if (type.Contains("witch")) return witch;
        if (type.Contains("cat")) return cat;

        return null;
    }

    private void ResetHealth(GameObject charObj)
    {
        if (charObj != null)
        {
            Entity e = charObj.GetComponent<Entity>();
            if (e != null) e.health = e.maxHealth;
        }
    }

    private void ApplyHealth(GameObject charObj, int hp)
    {
        if (charObj != null)
        {
            Entity e = charObj.GetComponent<Entity>();
            if (e != null) e.health = hp;
        }
    }
}