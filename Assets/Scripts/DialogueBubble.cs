using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueBubble : MonoBehaviour
{
    public GameObject root;
    public TMP_Text textUI;
    public Button continueButton;
    public float letterDelay = 0.04f;

    private bool isFinished;

    private void Awake()
    {
        continueButton.gameObject.SetActive(false);
    }

    public IEnumerator ShowText(string message)
    {
        root.SetActive(true);
        textUI.text = "";
        continueButton.gameObject.SetActive(false);
        isFinished = false;

        foreach (char c in message)
        {
            textUI.text += c;
            yield return new WaitForSeconds(letterDelay);
        }

        isFinished = true;
        continueButton.gameObject.SetActive(true);
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}
