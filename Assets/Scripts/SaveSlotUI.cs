using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SaveSlotUI : MonoBehaviour
{
    public int slotNumber = 1;
    public TextMeshProUGUI infoText;
    public Button button;

    private void Start()
    {
        UpdateSlotInfo();
    }

    public void UpdateSlotInfo()
    {
        SaveData data = DatabaseManager.instance.LoadGame(slotNumber);

        if (data != null)
        {
            int displayHealth = 0;
            if (data.PlayerType.Contains("Knight"))
                displayHealth = data.KnightHealth;
            else if (data.PlayerType.Contains("Witch"))
                displayHealth = data.WitchHealth;
            else if (data.PlayerType.Contains("Cat"))
                displayHealth = data.CatHealth;
            else
                displayHealth = data.KnightHealth;

            infoText.text = $"{data.PlayerType} | HP: {displayHealth}\n{data.SaveDate}";
        }
        else
        {
            infoText.text = $"Слот {slotNumber}\nПусто";
        }
    }

    public void OnSlotClicked()
    {
        GameSession.CurrentSlotIndex = slotNumber;

        if (!GameSession.IsNewGame)
        {
            SaveData data = DatabaseManager.instance.LoadGame(slotNumber);
            if (data != null)
            {
                SceneManager.LoadScene(data.SceneName);
            }
            else
            {
                Debug.Log("Слот пуст, нельзя загрузить!");
            }
        }
        else
        {
            SceneManager.LoadScene("PlayScene");
        }
    }
}