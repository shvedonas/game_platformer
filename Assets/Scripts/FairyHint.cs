using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class FairyHint : MonoBehaviour
{
    private bool isPlaying;
    public void Play(string hintText, FairyMovement fairy, DialogueBubble bubble)
    {
        if (isPlaying) return;
        StartCoroutine(HintRoutine(hintText, fairy, bubble));
    }
    private IEnumerator HintRoutine(string hintText, FairyMovement fairy, DialogueBubble bubble)
    {
        isPlaying = true;
        Entity player = SwitchCharacter.ActiveCharacter.GetComponent<Entity>();
        player.LockInput(true);

        fairy.gameObject.SetActive(true);
        fairy.transform.position = fairy.pointA.position;
        yield return fairy.FlyTo(fairy.pointB);

        yield return bubble.ShowText(hintText);

        bool clicked = false;
        bubble.continueButton.onClick.RemoveAllListeners();
        bubble.continueButton.onClick.AddListener(() => clicked = true);
        yield return new WaitUntil(() => clicked);

        bubble.Hide();

        yield return fairy.FlyTo(fairy.pointA);
        fairy.gameObject.SetActive(false);

        player.LockInput(false);

        isPlaying = false;
    }
}
