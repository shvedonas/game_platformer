using SQLite4Unity3d;

public class InventoryItemDB
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; } 

    [Indexed]
    public int SaveSlotId { get; set; } 

    public int SlotIndex { get; set; }  
    public string ItemId { get; set; }  
    public int Count { get; set; }     

    public InventoryItemDB() { }

    public InventoryItemDB(int saveSlotId, int slotIndex, string itemId, int count)
    {
        SaveSlotId = saveSlotId;
        SlotIndex = slotIndex;
        ItemId = itemId;
        Count = count;
    }
}