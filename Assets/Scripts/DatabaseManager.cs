using SQLite4Unity3d;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class DestroyedObjectDB
{
    [PrimaryKey]
    public string UniqueId { get; set; }
    public int SaveSlotId { get; set; }
}

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;
    private SQLiteConnection _connection;
    private string _dbName = "GameSave.db";

    private HashSet<string> _pendingDestroyedObjects = new HashSet<string>();

    public static HashSet<string> ShownHints = new HashSet<string>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            string dbPath = Path.Combine(Application.persistentDataPath, _dbName);
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            _connection.CreateTable<SaveData>();
            _connection.CreateTable<InventoryItemDB>();
            _connection.CreateTable<DestroyedObjectDB>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SaveGame(SaveData data)
    {
        _connection.InsertOrReplace(data);
        Debug.Log($"Игра сохранена в слот {data.SlotId}");
    }

    public SaveData LoadGame(int slotId)
    {
        return _connection.Table<SaveData>().Where(x => x.SlotId == slotId).FirstOrDefault();
    }

    public bool IsSlotEmpty(int slotId)
    {
        var data = LoadGame(slotId);
        return data == null;
    }

    private void OnApplicationQuit()
    {
        _connection?.Close();
    }

    public void SaveInventory(int slotId, List<InventoryItemDB> items)
    {
        _connection.Execute("DELETE FROM InventoryItemDB WHERE SaveSlotId = ?", slotId);
        _connection.InsertAll(items);
        Debug.Log("Инвентарь сохранен!");
    }

    public List<InventoryItemDB> LoadInventory(int slotId)
    {
        return _connection.Table<InventoryItemDB>()
                          .Where(x => x.SaveSlotId == slotId)
                          .ToList();
    }

    public void AddDestroyedObject(string uniqueId)
    {
        var obj = new DestroyedObjectDB
        {
            UniqueId = uniqueId + "_" + GameSession.CurrentSlotIndex,
            SaveSlotId = GameSession.CurrentSlotIndex
        };
        _connection.InsertOrReplace(obj);
    }

    public void ClearSaveSlot(int slotId)
    {
        try
        {
            ClearPendingObjects();
            ClearHints();
            _connection.Execute("DELETE FROM SaveData WHERE SlotId = ?", slotId);
            _connection.Execute("DELETE FROM InventoryItemDB WHERE SaveSlotId = ?", slotId);
            _connection.Execute("DELETE FROM DestroyedObjectDB WHERE SaveSlotId = ?", slotId);

            Debug.Log($"[Database] Слот {slotId} очищен.");

        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Ошибка при очистке слота: {e.Message}.");
        }
    }

    public void MarkAsDestroyedTemporary(string uniqueId)
    {
        string fullId = uniqueId + "_" + GameSession.CurrentSlotIndex;
        if (!_pendingDestroyedObjects.Contains(fullId))
        {
            _pendingDestroyedObjects.Add(fullId);
            Debug.Log($"Объект {fullId} добавлен в PENDING.");
        }
    }

    public void CommitDestroyedObjects()
    {
        foreach (string fullId in _pendingDestroyedObjects)
        {
            var obj = new DestroyedObjectDB
            {
                UniqueId = fullId,
                SaveSlotId = GameSession.CurrentSlotIndex
            };
            _connection.InsertOrReplace(obj);
        }
    }

    public void ClearPendingObjects()
    {
        _pendingDestroyedObjects.Clear();
    }

    public void ClearHints()
    {
        ShownHints.Clear();
    }

    public bool IsObjectDestroyed(string uniqueId)
    {
        string fullId = uniqueId + "_" + GameSession.CurrentSlotIndex;

        if (_pendingDestroyedObjects.Contains(fullId)) return true;

        var obj = _connection.Find<DestroyedObjectDB>(fullId);
        return obj != null;
    }
}