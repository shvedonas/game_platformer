using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationUI : MonoBehaviour
{
    public static NotificationUI Instance;

    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private TextMeshProUGUI notificationText;

    private void Awake()
    {
        Instance = this;
        if (notificationPanel) notificationPanel.SetActive(false);
    }

    public void ShowNotification(string message, float duration = 2.0f)
    {
        notificationPanel.SetActive(true);
        notificationText.text = message;
        StopAllCoroutines();
        StartCoroutine(HideRoutine(duration));
    }

    private IEnumerator HideRoutine(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        notificationPanel.SetActive(false);
    }
}