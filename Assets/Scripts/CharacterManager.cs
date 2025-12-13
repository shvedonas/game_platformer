using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCharacter : MonoBehaviour
{
    public static GameObject ActiveCharacter { get; private set; }

    [Header("Персонажи")]
    [SerializeField] private GameObject knight;
    [SerializeField] private GameObject witch;
    [SerializeField] private GameObject cat;

    [Header("Настройки")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private GameObject currentCharacter;

    private void Start()
    {
        Time.timeScale = 1.0f;

        if (knight) knight.SetActive(false);
        if (witch) witch.SetActive(false);
        if (cat) cat.SetActive(false);

        string characterTypeToLoad = "Witch";
        Vector3 positionToLoad = Vector3.zero;

        if (startPoint != null)
            positionToLoad = startPoint.position;

        SaveData data = null;

        if (!GameSession.IsNewGame)
        {
            data = DatabaseManager.instance.LoadGame(GameSession.CurrentSlotIndex);
        }

        if (data != null)
        {
            Debug.Log($"[SwitchCharacter] Загрузка сохранения. Персонаж: {data.PlayerType}");
            characterTypeToLoad = data.PlayerType;
            positionToLoad = new Vector3(data.PositionX, data.PositionY, data.PositionZ);
        }
        else
        {
            Debug.Log($"[SwitchCharacter] Новая игра. Выбран: {GameSession.SelectedCharacterType}");
            if (!string.IsNullOrEmpty(GameSession.SelectedCharacterType))
            {
                characterTypeToLoad = GameSession.SelectedCharacterType;
            }
        }

        GameObject targetInfo = GetCharacterObjectByType(characterTypeToLoad);

        if (targetInfo == null) targetInfo = witch != null ? witch : knight;

        targetInfo.transform.position = positionToLoad;

        if (data != null)
        {
            Entity entity = targetInfo.GetComponent<Entity>();
            if (entity != null)
            {
                entity.health = data.Health;
                entity.damage = data.Damage;
            }
        }

        ActivateCharacter(targetInfo);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1)) SwitchC(knight);
        else if (Input.GetKeyUp(KeyCode.Alpha2)) SwitchC(witch);
        else if (Input.GetKeyUp(KeyCode.Alpha3)) SwitchC(cat);
    }

    private void ActivateCharacter(GameObject character)
    {
        if (character == null) return;

        if (currentCharacter != null)
        {
            currentCharacter.SetActive(false);
        }

        currentCharacter = character;
        currentCharacter.SetActive(true);
        ActiveCharacter = currentCharacter;

        if (virtualCamera != null)
        {
            virtualCamera.Follow = currentCharacter.transform;
        }
    }

    private void SwitchC(GameObject targetCharacter)
    {
        if (targetCharacter == null || targetCharacter == currentCharacter) return;

        Vector3 lastPosition = currentCharacter.transform.position;

        BoxCollider2D newCollider = targetCharacter.GetComponent<BoxCollider2D>();
        if (newCollider != null)
        {
            Vector2 checkSize = newCollider.size * Mathf.Max(targetCharacter.transform.localScale.x, targetCharacter.transform.localScale.y);
            Vector2 checkCenter = (Vector2)lastPosition + newCollider.offset + (Vector2.up * 0.1f);

            LayerMask groundMask = LayerMask.GetMask("Ground");
            Collider2D hit = Physics2D.OverlapBox(checkCenter, checkSize, 0f, groundMask);

            if (hit != null)
            {
                Debug.Log($"Нельзя переключиться на {targetCharacter.name} — мало места!");
                return;
            }
        }

        targetCharacter.transform.position = lastPosition;

        ActivateCharacter(targetCharacter);

        Debug.Log($"Переключились на: {targetCharacter.name}");
    }

    private GameObject GetCharacterObjectByType(string typeName)
    {
        string type = typeName.ToLower();

        if (type.Contains("knight")) return knight;
        if (type.Contains("witch")) return witch;
        if (type.Contains("cat")) return cat;

        return null;
    }
}