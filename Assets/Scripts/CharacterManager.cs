using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCharacter : MonoBehaviour
{
    public static GameObject ActiveCharacter { get; private set; }
    [SerializeField] public GameObject knight;
    [SerializeField] public GameObject cat;
    [SerializeField] public GameObject witch;

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
            Vector2 checkSize = newCollider.size * Mathf.Max(character.transform.localScale.x, character.transform.localScale.y);
            Vector2 checkCenter = lastPosition + Vector3.up * (checkSize.y / 2f);
            LayerMask groundMask = LayerMask.GetMask("Ground");
            Collider2D hit = Physics2D.OverlapBox(checkCenter, checkSize, 0f, groundMask);

            if (hit != null)
            {
                Debug.Log($"Нельзя переключиться на {character.name} — персонаж не помещается!");
                return;
            }
        }

        currentCharacter.SetActive(false);
        character.transform.position = lastPosition;
        currentCharacter = character;
        currentCharacter.SetActive(true);
        ActiveCharacter = currentCharacter;
        virtualCamera.Follow = currentCharacter.transform;

        Debug.Log($"Переключились на: {character.name}");
    }

}