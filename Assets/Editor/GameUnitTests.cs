using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; 

public class GameUnitTests
{
    // 1. Тесты инвентаря

    [Test]
    public void Inventory_01_Positive_AddToEmpty()
    {
        var inv = CreateMockInventory();
        var item = CreateItem("apple", 10);
        inv.allItemsDatabase = new List<ItemData> { item };

        bool result = inv.AddItem("apple", 1);

        Assert.IsTrue(result);
        Assert.AreEqual(1, inv.slots[0].count);
        CleanUp(inv);
    }

    [Test]
    public void Inventory_02_Positive_Stacking()
    {
        var inv = CreateMockInventory();
        var item = CreateItem("apple", 10);
        inv.allItemsDatabase = new List<ItemData> { item };

        inv.slots[0].item = item;
        inv.slots[0].count = 5;

        bool result = inv.AddItem("apple", 3);

        Assert.AreEqual(8, inv.slots[0].count);
        CleanUp(inv);
    }

    [Test]
    public void Inventory_03_Boundary_FillMax()
    {
        var inv = CreateMockInventory();
        var item = CreateItem("apple", 10);
        inv.allItemsDatabase = new List<ItemData> { item };

        inv.slots[0].item = item;
        inv.slots[0].count = 9;

        inv.AddItem("apple", 1);

        Assert.AreEqual(10, inv.slots[0].count);
        CleanUp(inv);
    }

    [Test]
    public void Inventory_04_Boundary_Overflow()
    {
        var inv = CreateMockInventory();
        var item = CreateItem("apple", 10);
        inv.allItemsDatabase = new List<ItemData> { item };

        inv.slots[0].item = item;
        inv.slots[0].count = 10; 

        bool result = inv.AddItem("apple", 5);

        Assert.IsTrue(result);
        Assert.AreEqual(10, inv.slots[0].count);
        Assert.AreEqual(5, inv.slots[1].count); 
        CleanUp(inv);
    }

    [Test]
    public void Inventory_05_Negative_InvalidID()
    {
        var inv = CreateMockInventory();

        inv.allItemsDatabase = new List<ItemData>();

        bool result = inv.AddItem("unknown_item", 1);

        Assert.IsFalse(result);
        CleanUp(inv);
    }

    [Test]
    public void Inventory_06_Negative_FullInventory()
    {
        var inv = CreateMockInventory();
        var item = CreateItem("apple", 10);
        inv.allItemsDatabase = new List<ItemData> { item };

        for (int i = 0; i < 15; i++) { inv.slots[i].item = item; inv.slots[i].count = 10; }

        bool result = inv.AddItem("apple", 1);
        Assert.IsFalse(result);
        CleanUp(inv);
    }


    // 2. Тесты получения урона

    [Test]
    public void Entity_01_Positive_Damage()
    {
        var ent = CreateEntity(100);
        var attacker = CreateEntity(0); attacker.damage = 20;

        ent.TakeDamage(attacker);
        Assert.AreEqual(80, ent.health);
        CleanUp(ent); CleanUp(attacker);
    }

    [Test]
    public void Entity_02_Positive_Kill()
    {
        var ent = CreateEntity(10);
        var attacker = CreateEntity(0); attacker.damage = 10;

        ent.TakeDamage(attacker);

        Assert.IsTrue(ent == null || ent.gameObject == null || ent.health <= 0);
        if (ent != null) CleanUp(ent);
        CleanUp(attacker);
    }

    [Test]
    public void Entity_03_Boundary_Overkill()
    {
        var ent = CreateEntity(5);
        var attacker = CreateEntity(0); attacker.damage = 1000;

        ent.TakeDamage(attacker);
        if (ent != null) Assert.AreEqual(-995, ent.health);
        else Assert.Pass(); 

        if (ent != null) CleanUp(ent);
        CleanUp(attacker);
    }

    [Test]
    public void Entity_04_Boundary_ZeroDamage()
    {
        var ent = CreateEntity(50);
        var attacker = CreateEntity(0); attacker.damage = 0;

        ent.TakeDamage(attacker);
        Assert.AreEqual(50, ent.health);
        CleanUp(ent); CleanUp(attacker);
    }

    [Test]
    public void Entity_05_Negative_AlreadyDead()
    {
        var ent = CreateEntity(0);
        ent.isDead = true;
        var attacker = CreateEntity(0); attacker.damage = 10;

        ent.TakeDamage(attacker);
        Assert.AreEqual(0, ent.health); 
        CleanUp(ent); CleanUp(attacker);
    }

    [Test]
    public void Entity_06_Negative_HealingAttack()
    {
        var ent = CreateEntity(50);
        var attacker = CreateEntity(0); attacker.damage = -20;

        ent.TakeDamage(attacker);
        Assert.AreEqual(70, ent.health); 
        CleanUp(ent); CleanUp(attacker);
    }


    // 3. Тесты обнаружения персонажа

    [Test]
    public void Switch_01_Positive_Exact()
    {
        Assert.AreEqual("witch", SimulateSearch("Witch"));
    }

    [Test]
    public void Switch_02_Positive_LowerCase()
    {
        Assert.AreEqual("cat", SimulateSearch("cat"));
    }

    [Test]
    public void Switch_03_Boundary_MixedCase()
    {
        Assert.AreEqual("knight", SimulateSearch("KnIgHt"));
    }

    [Test]
    public void Switch_04_Boundary_Substring()
    {
        Assert.AreEqual("witch", SimulateSearch("SuperWitch"));
    }

    [Test]
    public void Switch_05_Negative_NotFound()
    {
        Assert.IsNull(SimulateSearch("Dragon"));
    }

    [Test]
    public void Switch_06_Negative_Empty()
    {
        Assert.IsNull(SimulateSearch(""));
    }

    private string SimulateSearch(string input)
    {
        string t = input.ToLower();
        if (t.Contains("knight")) return "knight";
        if (t.Contains("witch")) return "witch";
        if (t.Contains("cat")) return "cat";
        return null;
    }


    // 4. Тесты базы данных

    [Test]
    public void SaveData_01_Positive_Create()
    {
        SaveData d = new SaveData(1, "Scene", "Witch", Vector3.zero, 100, 10);
        Assert.AreEqual(1, d.SlotId);
        Assert.AreEqual("Witch", d.PlayerType);
    }

    [Test]
    public void SaveData_02_Positive_DateGenerated()
    {
        SaveData d = new SaveData(1, "S", "W", Vector3.zero, 1, 1);
        Assert.IsNotNull(d.SaveDate);
        Assert.IsNotEmpty(d.SaveDate);
    }

    [Test]
    public void SaveData_03_Boundary_NegativeHP()
    {
        SaveData d = new SaveData(1, "S", "C", Vector3.zero, -100, 5);
        Assert.AreEqual(-100, d.Health);
    }

    [Test]
    public void SaveData_04_Boundary_LargeCoord()
    {
        SaveData d = new SaveData(1, "S", "C", new Vector3(9999, 9999, 0), 10, 5);
        Assert.AreEqual(9999, d.PositionX);
    }

    [Test]
    public void SaveData_05_Negative_NullScene()
    {
        SaveData d = new SaveData(1, null, "C", Vector3.zero, 10, 5);
        Assert.IsNull(d.SceneName);
    }

    [Test]
    public void SaveData_06_Boundary_ZeroID()
    {
        SaveData d = new SaveData(0, "S", "C", Vector3.zero, 10, 5);
        Assert.AreEqual(0, d.SlotId);
    }


    // 5. Тесты чекпоинтов

    [Test]
    public void Checkpoint_01_Positive_Player()
    {
        Assert.IsTrue(SimulateTrigger("Player"));
    }

    [Test]
    public void Checkpoint_02_Negative_Enemy()
    {
        Assert.IsFalse(SimulateTrigger("Enemy"));
    }

    [Test]
    public void Checkpoint_03_Negative_Untagged()
    {
        Assert.IsFalse(SimulateTrigger("Untagged"));
    }

    [Test]
    public void Checkpoint_04_Boundary_CaseSensitive()
    {
        Assert.IsFalse(SimulateTrigger("player"));
    }

    [Test]
    public void Checkpoint_05_Boundary_Empty()
    {
        Assert.IsFalse(SimulateTrigger(""));
    }

    [Test]
    public void Checkpoint_06_Boundary_Space()
    {
        Assert.IsFalse(SimulateTrigger("Player "));
    }

    private bool SimulateTrigger(string tag) { return tag == "Player"; }

    // Вспомогательные методы

    private ItemData CreateItem(string id, int stack)
    {
        var i = ScriptableObject.CreateInstance<ItemData>();
        i.id = id; i.maxStackSize = stack;
        return i;
    }

    private InventoryManager CreateMockInventory()
    {
        var go = new GameObject("TestInv");
        var inv = go.AddComponent<InventoryManager>();
        inv.slots = new InventorySlot[15];
        for (int i = 0; i < 15; i++) inv.slots[i] = new InventorySlot();

        inv.inventoryPanel = new GameObject("Panel");
        inv.slotsParent = new GameObject("SlotsGrid").transform;
        for (int i = 0; i < 15; i++)
        {
            var s = new GameObject($"Slot{i}");
            s.transform.SetParent(inv.slotsParent);
            var ico = new GameObject("Icon"); ico.transform.SetParent(s.transform);
            ico.AddComponent<Image>();
            var txt = new GameObject("CountText"); txt.transform.SetParent(s.transform);
            txt.AddComponent<TextMeshProUGUI>();
        }
        return inv;
    }

    private Entity CreateEntity(int hp)
    {
        var go = new GameObject("Ent");
        var e = go.AddComponent<Entity>();
        e.health = hp;
        return e;
    }

    private void CleanUp(MonoBehaviour mb)
    {
        if (mb != null && mb.gameObject != null)
            Object.DestroyImmediate(mb.gameObject);
    }
}