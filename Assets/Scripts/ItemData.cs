using UnityEngine;

public enum ItemType
{
    Food,       
    Ingredient,
    Upgrade     
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string id;           
    public string itemName;     
    public Sprite icon;         
    public ItemType type;       
    [TextArea] public string description; 
    public int maxStackSize = 1;


}