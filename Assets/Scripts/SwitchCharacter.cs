using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCharacter : MonoBehaviour
{
    [SerializeField] private GameObject knight;
    [SerializeField] private GameObject cat;
    [SerializeField] private GameObject witch;

    private GameObject currentCharacter;
    private Vector3 lastPosition;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    void Start()
    {
        knight.SetActive(true);
        cat.SetActive(false);
        witch.SetActive(false);
        currentCharacter = knight;

        switchC(currentCharacter);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            switchC(knight);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            switchC(witch);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            switchC(cat);
        }
    }

    private void switchC(GameObject character)
    {
        if (character == currentCharacter) return;

        lastPosition = currentCharacter.transform.position;

        BoxCollider2D newCollider = character.GetComponent<BoxCollider2D>();

        if (newCollider != null)
        {
            // Центр проверки с учётом offset коллайдера
            

            // Размер с учётом масштаба
            Vector2 checkSize = newCollider.size * Mathf.Max(character.transform.localScale.x, character.transform.localScale.y);
            Vector2 checkCenter = lastPosition + Vector3.up * (checkSize.y / 2f);
            // Проверяем пересечения с землёй
            LayerMask groundMask = LayerMask.GetMask("Ground");
            Collider2D hit = Physics2D.OverlapBox(checkCenter, checkSize, 0f, groundMask);

            if (hit != null)
            {
                Debug.Log($"Нельзя переключиться на {character.name} — персонаж не помещается!");
                return;
            }
        }

        // Если место свободно — переключаем
        currentCharacter.SetActive(false);
        character.transform.position = lastPosition;
        currentCharacter = character;
        currentCharacter.SetActive(true);
        virtualCamera.Follow = currentCharacter.transform;

        Debug.Log($"Переключились на: {character.name}");
    }
}