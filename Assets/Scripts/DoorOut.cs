using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class DoorOut : MonoBehaviour
{
    [Header("Настройки двери")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Collider2D physicsCollider;
    [SerializeField] private SpriteRenderer sprite;
    private bool openedFromCheckpoint = false;
    private bool isDoorOpen = false;
    public UniqueObject uniqueObj;
    void Start()
    {
        if (!GameSession.IsNewGame &&
        DatabaseManager.instance.IsObjectDestroyed(uniqueObj.uniqueId))
        {
            openedFromCheckpoint = true;
            SetOpenStateImmediate();
        }
    }

    void Update()
    {

    }
    public void SetOpenStateImmediate()
    {
    isDoorOpen = true;
    if (physicsCollider != null) physicsCollider.enabled = false;
    if (sprite != null) sprite.enabled = false;
    if (doorAnimator != null) doorAnimator.Play("Open", 0, 1.0f);
    }
    public void ResetDoorIfNotSaved()
    {
        if (!openedFromCheckpoint)
        {
            isDoorOpen = false;
            if (physicsCollider) physicsCollider.enabled = true;
            if (sprite) sprite.enabled = true;
            if (doorAnimator) doorAnimator.Play("Closed", 0, 0f);
        }
    }

}
