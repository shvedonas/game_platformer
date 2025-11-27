using SQLite4Unity3d;
using System;

public class SaveData
{
    [PrimaryKey]
    public int SlotId { get; set; }

    public string SceneName { get; set; }

    public string PlayerType { get; set; }

    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float PositionZ { get; set; }

    public int Health { get; set; }
    public int Damage { get; set; }

    public string SaveDate { get; set; }

    public SaveData() { }
    public SaveData(int slotId, string sceneName, string playerType, UnityEngine.Vector3 pos, int hp, int dmg)
    {
        SlotId = slotId;
        SceneName = sceneName;
        PlayerType = playerType;
        PositionX = pos.x;
        PositionY = pos.y;
        PositionZ = pos.z;
        Health = hp;
        Damage = dmg;
        SaveDate = DateTime.Now.ToString("g"); 
    }
}