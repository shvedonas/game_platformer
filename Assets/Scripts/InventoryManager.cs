using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int count;

    public void Clear()
    {
        item = null;
        count = 0;
    }
}
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("UI")]
    public GameObject inventoryPanel;
    public Transform slotsParent;

    [Header("Database")]
    public List<ItemData> allItemsDatabase;

    public InventorySlot[] slots = new InventorySlot[15];

    private bool isOpen = false;

    private void Awake()
    {
        instance = this;
        inventoryPanel.SetActive(false);
        for (int i = 0; i < slots.Length; i++) slots[i] = new InventorySlot();
    }

    private void Start()
    {
        LoadInventoryFromDB();
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isOpen = !isOpen;
            inventoryPanel.SetActive(isOpen);
            if (isOpen) UpdateUI();
        }
    }

    public bool AddItem(string itemId, int amount = 1)
    {
        ItemData data = allItemsDatabase.Find(x => x.id == itemId);
        if (data == null) return false;

        if (data.maxStackSize > 1)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == data && slots[i].count < data.maxStackSize)
                {
                    int spaceLeft = data.maxStackSize - slots[i].count;

                    if (amount <= spaceLeft)
                    {
                        slots[i].count += amount;
                        UpdateUI();
                        return true;
                    }
                    else
                    {
                        slots[i].count = data.maxStackSize;
                        amount -= spaceLeft;
                    }
                }
            }
        }

        while (amount > 0)
        {
            int emptySlotIndex = -1;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == null)
                {
                    emptySlotIndex = i;
                    break;
                }
            }

            if (emptySlotIndex != -1)
            {
                slots[emptySlotIndex].item = data;
                int toAdd = Mathf.Min(amount, data.maxStackSize);
                slots[emptySlotIndex].count = toAdd;
                amount -= toAdd;
            }
            else
            {
                Debug.Log("Инвентарь полон!");
                UpdateUI();
                return false;
            }
        }

        UpdateUI();
        return true;
    }

    public void SaveInventoryToDB()
    {
        List<InventoryItemDB> dbList = new List<InventoryItemDB>();
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                InventoryItemDB item = new InventoryItemDB(
                    GameSession.CurrentSlotIndex,
                    i,
                    slots[i].item.id,
                    slots[i].count 
                );
                dbList.Add(item);
            }
        }
        DatabaseManager.instance.SaveInventory(GameSession.CurrentSlotIndex, dbList);
    }

    public void LoadInventoryFromDB()
    {
        for (int i = 0; i < slots.Length; i++) slots[i].Clear();

        if (GameSession.IsNewGame) return;

        var dbItems = DatabaseManager.instance.LoadInventory(GameSession.CurrentSlotIndex);
        foreach (var dbItem in dbItems)
        {
            ItemData data = allItemsDatabase.Find(x => x.id == dbItem.ItemId);
            if (data != null && dbItem.SlotIndex < slots.Length)
            {
                slots[dbItem.SlotIndex].item = data;
                slots[dbItem.SlotIndex].count = dbItem.Count;
            }
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < slotsParent.childCount; i++)
        {
            if (i >= slots.Length) break;

            Transform slotObj = slotsParent.GetChild(i);
            Transform iconTr = slotObj.Find("Icon");

            TextMeshProUGUI countText = slotObj.Find("CountText").GetComponent<TextMeshProUGUI>();

            Image iconImage = iconTr.GetComponent<Image>();

            if (slots[i].item != null)
            {
                iconImage.sprite = slots[i].item.icon;
                iconImage.color = Color.white;
                iconImage.preserveAspect = true;
                iconImage.enabled = true;

                if (slots[i].count > 0)
                {
                    countText.text = slots[i].count.ToString();
                    countText.enabled = true;
                }
                else
                {
                    countText.enabled = false;
                }
            }
            else
            {
                iconImage.sprite = null;
                iconImage.color = Color.clear;
                iconImage.enabled = false;
                countText.enabled = false;
            }
        }
    }
}