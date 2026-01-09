using UnityEngine;
using System.Collections;
using TMPro;

public class FairyController : MonoBehaviour
{
    [Header("Маршрут")]
    public Transform pointA;
    public Transform pointB; 
    public Transform pointC; 
    public Transform pointD; 

    [Header("Настройки")]
    public float speed = 5f;
    public float typingSpeed = 0.05f;

    [Header("UI")]
    public GameObject dialoguePanel;      
    public TextMeshProUGUI dialogueText;  
    public GameObject continueButton;    
    [TextArea] public string helpMessage = "Давай помогу, беги за мной!";

    private Collider2D myCollider;
    private SpriteRenderer mySprite;
    private bool isWaitingForPlayer = false; 

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        mySprite = GetComponent<SpriteRenderer>();
        
    }

    public void StartFairySequence()
    {
        gameObject.SetActive(true);
        transform.position = pointA.position; 
        Entity player = SwitchCharacter.ActiveCharacter.GetComponent<Entity>();
        if (player != null) player.LockInput(true);

        StartCoroutine(FlyToB_Routine());
    }

    private IEnumerator FlyToB_Routine()
    {
        yield return MoveToTarget(pointB.position);

        yield return ShowDialogue();
    }

    private IEnumerator ShowDialogue()
    {
        if (dialoguePanel) dialoguePanel.SetActive(true);
        if (continueButton) continueButton.SetActive(false); 
        
        dialogueText.text = "";
        foreach (char letter in helpMessage.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (continueButton) continueButton.SetActive(true);
    }

    public void OnContinuePressed()
    {
        if (dialoguePanel) dialoguePanel.SetActive(false);

        Entity player = SwitchCharacter.ActiveCharacter.GetComponent<Entity>();
        if (player != null) player.LockInput(false);

        StartCoroutine(FlyToC_Routine());
    }

    private IEnumerator FlyToC_Routine()
    {
        yield return MoveToTarget(pointC.position);

        if (myCollider) myCollider.enabled = true;
        isWaitingForPlayer = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isWaitingForPlayer && collision.CompareTag("Player"))
        {
            isWaitingForPlayer = false;
            if (myCollider) myCollider.enabled = false; 
            StartCoroutine(FlyToD_Routine());
        }
    }

    private IEnumerator FlyToD_Routine()
    {
        yield return MoveToTarget(pointD.position);
        gameObject.SetActive(false);
    }
    private IEnumerator MoveToTarget(Vector3 target)
    {
        if (target.x < transform.position.x)
            transform.localScale = new Vector3(-4, 4, 4); 
        else
            transform.localScale = new Vector3(4, 4, 4);  

        while (Vector2.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }
}