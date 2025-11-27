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
            infoText.text = $"{data.PlayerType} | HP: {data.Health}\n{data.SaveDate}";
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