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
        if (playOnce && DatabaseManager.instance.IsObjectDestroyed(uniqueObj.uniqueId))
        {
            SwitchCharacter.Instance.catActive = true;
            SwitchCharacter.Instance.witchActive = true;
            return;
        }
        if (triggered && playOnce) return;

            if (collision.CompareTag("Player"))
            {
                if (fairy.name == "Fairy1") SwitchCharacter.Instance.catActive = true;
                if (fairy.name == "Fairy3") SwitchCharacter.Instance.witchActive = true;
                triggered = true;
                hint.Play(hintText, fairy, bubble);
                DatabaseManager.instance.MarkAsDestroyedTemporary(uniqueObj.uniqueId);
            }
        
    }
}
