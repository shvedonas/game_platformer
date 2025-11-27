using UnityEngine;
using SQLite4Unity3d;
using System.IO;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;
    private SQLiteConnection _connection;
    private string _dbName = "GameSave.db";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            string dbPath = Path.Combine(Application.persistentDataPath, _dbName);
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            _connection.CreateTable<SaveData>();
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
}