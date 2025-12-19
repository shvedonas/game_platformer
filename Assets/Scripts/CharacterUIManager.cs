using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterUIManager : MonoBehaviour
{
    [Header("Hearts")]
    public Transform heartsParent;
    public GameObject heartPrefab;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Character Frames")]
    public GameObject knightFrame;
    public GameObject witchFrame;
    public GameObject catFrame;

    private List<GameObject> hearts = new List<GameObject>();
    private Entity currentEntity;

    public static CharacterUIManager Instance;

    private void Awake()
    {
        Instance = this;
    }


    public void SetCharacter(Entity entity)
    {
        currentEntity = entity;

        UpdateHearts(entity.health, entity.maxHealth);
        UpdateFrames(entity.gameObject);
    }

    public void UpdateHearts(int current, int max)
    {
        foreach (var h in hearts)
            Destroy(h);
        hearts.Clear();

        for (int i = 0; i < max; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartsParent);
            Image img = heart.GetComponent<Image>();

            img.sprite = i < current ? fullHeart : emptyHeart;
            img.enabled = true;

            hearts.Add(heart);
        }
    }

    public void UpdateFrames(GameObject activeCharacter)
    {
        knightFrame.SetActive(false);
        witchFrame.SetActive(false);
        catFrame.SetActive(false);

        if (activeCharacter.GetComponent<Knight>() != null)
            knightFrame.SetActive(true);
        else if (activeCharacter.GetComponent<Witch>() != null)
            witchFrame.SetActive(true);
        else if (activeCharacter.GetComponent<Cat>() != null)
            catFrame.SetActive(true);
    }

    public void OnHealthChanged(int newHealth)
    {
        UpdateHearts(newHealth, currentEntity.maxHealth);
    }
}
