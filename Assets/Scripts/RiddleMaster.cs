using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class RiddleData
{
    [TextArea(2, 5)]
    public string questionText;
    public string[] answers = new string[4];
    public int correctAnswerIndex;
}

[RequireComponent(typeof(UniqueObject))]
public class RiddleMaster : MonoBehaviour
{
    public bool open = false;
    [Header("Настройки")]
    [SerializeField] private RiddleData[] riddles;
    [SerializeField] private string trialSceneName = "Examination";
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Настройки Облачка (Приветствие)")]
    [SerializeField] private GameObject bubblePanel; 
    [SerializeField] private TextMeshProUGUI bubbleText; 
    [SerializeField] private GameObject bubbleButtons;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button laterButton;
    [SerializeField] private Transform bubbleAnchor; 
    [SerializeField] private Canvas canvas;          
    private RectTransform bubbleRect;

    [Header("Текст")]
    [TextArea][SerializeField] private string introText = "Готов ли ты ответить на мои загадки, путник?";
    [TextArea][SerializeField] private string completedText = "Ты уже доказал свою мудрость.";

    [Header("UI - Загадки (Центр экрана)")]
    [SerializeField] private GameObject riddleMainPanel; 
    [SerializeField] private TextMeshProUGUI questionTextUI;
    [SerializeField] private GameObject answerButtonsContainer;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private TextMeshProUGUI[] answerTexts;

    [Header("Кат-сцена")]
    [SerializeField] private CinemachineVirtualCamera doorVirtualCamera;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string doorOpenTrigger = "Open";
    [SerializeField] private float cutsceneDuration = 3.0f;
    [SerializeField] private DoorOut doorOut;
    private int currentRiddleIndex = 0;
    private bool isCutscenePlaying = false;
    private bool isTyping = false;
    private UniqueObject uniqueObj;
    private Camera mainCam;

    private void Start()
    {
        if (bubblePanel) bubblePanel.SetActive(false);
        if (riddleMainPanel) riddleMainPanel.SetActive(false);
        if (doorVirtualCamera) doorVirtualCamera.gameObject.SetActive(false);

        if (yesButton) yesButton.onClick.AddListener(OnYesClicked);
        if (laterButton) laterButton.onClick.AddListener(CloseAll);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].onClick.AddListener(() => OnAnswerClicked(index));
        }

        if (!GameSession.IsNewGame && DatabaseManager.instance.IsObjectDestroyed(uniqueObj.uniqueId))
        {
            if (doorOut != null) doorOut.SetOpenStateImmediate();
        }
    }

    private void Update()
    {
        if (!bubblePanel.activeSelf || bubbleAnchor == null)
            return;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(
            mainCam,
            bubbleAnchor.position
        );

        RectTransform canvasRect = canvas.transform as RectTransform;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPoint,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCam,
            out localPoint
        );

        bubbleRect.localPosition = localPoint;
    }
    private void Awake()
    {
        mainCam = Camera.main;
        uniqueObj = GetComponent<UniqueObject>();

        if (bubblePanel != null)
        {
            bubbleRect = bubblePanel.GetComponent<RectTransform>();
            if (!canvas) canvas = bubblePanel.GetComponentInParent<Canvas>();
        }
    }

    public void StartInteraction()
    {
        if (isCutscenePlaying) return;

        LockPlayer(true);

        if (!GameSession.IsNewGame && DatabaseManager.instance.IsObjectDestroyed(uniqueObj.uniqueId))
        {
            StartCoroutine(ShowBubbleSequence(completedText, false));
        }
        else
        {
            StartCoroutine(ShowBubbleSequence(introText, true));
        }
    }

    private IEnumerator ShowBubbleSequence(string text, bool showButtons)
    {
        bubblePanel.SetActive(true);
        bubbleButtons.SetActive(false); 
        riddleMainPanel.SetActive(false); 

        yield return StartCoroutine(TypeWriterEffect(bubbleText, text));

        if (showButtons)
        {
            bubbleButtons.SetActive(true); 
        }
        else
        {
            yield return new WaitForSeconds(1.3f);
            CloseAll();
        }
    }

    private void OnYesClicked()
    {
        bubblePanel.SetActive(false);
        StartRiddles();
    }

    private void StartRiddles()
    {
        riddleMainPanel.SetActive(true);
        currentRiddleIndex = 0;
        StartCoroutine(ShowRiddleRoutine(currentRiddleIndex));
    }

    private IEnumerator ShowRiddleRoutine(int index)
    {
        answerButtonsContainer.SetActive(false);

        if (index >= riddles.Length)
        {
            SuccessSequence();
            yield break;
        }

        RiddleData data = riddles[index];

        yield return StartCoroutine(TypeWriterEffect(questionTextUI, data.questionText));

        answerButtonsContainer.SetActive(true);
        foreach (var btn in answerButtons) btn.interactable = false;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < data.answers.Length)
            {
                yield return StartCoroutine(TypeWriterEffect(answerTexts[i], data.answers[i]));
                answerButtons[i].interactable = true;
            }
            else
            {
                answerTexts[i].text = "";
                answerButtons[i].interactable = false;
            }
        }
    }

    private IEnumerator TypeWriterEffect(TextMeshProUGUI textComponent, string fullText)
    {
        isTyping = true;
        textComponent.text = "";
        foreach (char letter in fullText.ToCharArray())
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    private void OnAnswerClicked(int buttonIndex)
    {
        if (isTyping) return;

        if (buttonIndex == riddles[currentRiddleIndex].correctAnswerIndex)
        {
            currentRiddleIndex++;
            StartCoroutine(ShowRiddleRoutine(currentRiddleIndex));
        }
        else
        {
            Debug.Log("Неправильно! Сохраняем состояние и отправляем на испытание.");

            if (SwitchCharacter.ActiveCharacter != null)
            {
                Entity playerEntity = SwitchCharacter.ActiveCharacter.GetComponent<Entity>();
                if (playerEntity != null)
                {
                    playerEntity.SaveEntityData();
                }
            }

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.SaveInventoryToDB();
            }

            StartCoroutine(FailSequence());
        }
    }
    private IEnumerator FailSequence()
    {
        riddleMainPanel.SetActive(false);

        bubblePanel.SetActive(true);
        bubbleButtons.SetActive(false);

        string failText = "Неправильно...\nПройди испытание и приходи.";

        yield return StartCoroutine(TypeWriterEffect(bubbleText, failText));

        yield return new WaitForSeconds(0.5f);

        if (SwitchCharacter.ActiveCharacter != null)
        {
            Entity playerEntity =
                SwitchCharacter.ActiveCharacter.GetComponent<Entity>();

            if (playerEntity != null)
                playerEntity.SaveEntityData();
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.SaveInventoryToDB();
        }

        SceneManager.LoadScene(trialSceneName);
    }
    private void SuccessSequence()
    {
        if (!GameSession.IsNewGame &&
            DatabaseManager.instance.IsObjectDestroyed(uniqueObj.uniqueId))
            return;

        riddleMainPanel.SetActive(false);

        DatabaseManager.instance.MarkAsDestroyedTemporary(uniqueObj.uniqueId);
        open = true;
        StartCoroutine(PlayDoorCutscene());

    }

    private void CloseAll()
    {
        bubblePanel.SetActive(false);
        riddleMainPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        LockPlayer(false);
    }

    private void LockPlayer(bool locked)
    {
        if (SwitchCharacter.ActiveCharacter != null)
        {
            Entity playerEntity = SwitchCharacter.ActiveCharacter.GetComponent<Entity>();
            if (playerEntity != null) playerEntity.LockInput(locked);
        }
    }

    private IEnumerator PlayDoorCutscene()
    {
        isCutscenePlaying = true;
        if (doorVirtualCamera != null) { doorVirtualCamera.Priority = 20; doorVirtualCamera.gameObject.SetActive(true); }
        yield return new WaitForSeconds(1.5f);
        if (doorAnimator != null) doorAnimator.SetTrigger(doorOpenTrigger);
        yield return new WaitForSeconds(cutsceneDuration);
        if (doorVirtualCamera != null) { doorVirtualCamera.Priority = 0; doorVirtualCamera.gameObject.SetActive(false); }
        yield return new WaitForSeconds(1.5f);
        LockPlayer(false);
        isCutscenePlaying = false;
        this.enabled = false;
    }
}