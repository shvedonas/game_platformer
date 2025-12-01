using UnityEngine;

public class UniqueObject : MonoBehaviour
{
    public string uniqueId;

    private void Start()
    {
        if (DatabaseManager.instance.IsObjectDestroyed(uniqueId))
        {
            Destroy(gameObject); 
        }
    }

    [ContextMenu("Generate ID")]
    private void GenerateId()
    {
        uniqueId = System.Guid.NewGuid().ToString();
    }
}