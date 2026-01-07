using UnityEngine;

public class UniqueObject : MonoBehaviour
{
    public string uniqueId;
    public bool autoDestroy = true;
    private void Start()
    {
        if (GameSession.IsNewGame)
        {
            return;
        }
        else if (DatabaseManager.instance.IsObjectDestroyed(uniqueId))
        {
            if (autoDestroy)
            {
                Destroy(gameObject);
            }
        }
    }

    [ContextMenu("Generate ID")]
    private void GenerateId()
    {
        uniqueId = System.Guid.NewGuid().ToString();
    }
}