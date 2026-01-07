using SQLite4Unity3d;
using System;
using static Unity.Burst.Intrinsics.X86.Avx;

public class SaveData
{
    [PrimaryKey]
    public int SlotId { get; set; }

    public string SceneName { get; set; }

    public string PlayerType { get; set; }

    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float PositionZ { get; set; }

    public int KnightHealth { get; set; }
    public int WitchHealth { get; set; }
    public int CatHealth { get; set; }
    public int Damage { get; set; }

    public string SaveDate { get; set; }

    public SaveData() { }
    public SaveData(int slotId, string sceneName, string playerType, UnityEngine.Vector3 pos, int kHp, int wHp, int cHp, int dmg)
    {
        SlotId = slotId;
        SceneName = sceneName;
        PlayerType = playerType;
        PositionX = pos.x;
        PositionY = pos.y;
        PositionZ = pos.z;
        KnightHealth = kHp;
        WitchHealth = wHp;
        CatHealth = cHp;
        Damage = dmg;
        SaveDate = DateTime.Now.ToString("g"); 
    }
}