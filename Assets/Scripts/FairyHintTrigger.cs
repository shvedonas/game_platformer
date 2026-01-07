using System.Xml;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class FairyHintTrigger : MonoBehaviour
{
    [Header("Hint Text")]
    [TextArea(2, 5)]
    public string hintText;
    public FairyHint hint;
    public bool playOnce = true;
    public FairyMovement fairy;
    public DialogueBubble bubble;
    private UniqueObject uniqueObj;
    private bool triggered;

    private void Awake()
    {
        uniqueObj = GetComponent<UniqueObject>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playOnce && DatabaseManager.ShownHints.Contains(uniqueObj.uniqueId))
            return;
        if (triggered && playOnce) return;

            if (collision.CompareTag("Player"))
            {
                triggered = true;
                hint.Play(hintText, fairy, bubble);
                DatabaseManager.ShownHints.Add(uniqueObj.uniqueId);
            }
        
    }
}
