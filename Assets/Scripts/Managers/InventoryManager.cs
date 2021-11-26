using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : BaseManager
{
    public static InventoryManager Instance;
    public event Action OnItemNotForund;
    public List<InventoryItem> inventoryItems;
    public Image itemPrefab;
    public Transform itemPlace;

    private void Awake()
    {
        Instance = this;
    }

    public override void LoadManager()
    {
        base.LoadManager();
        inventoryItems = new List<InventoryItem>();
    }

    public void AddItem(InventoryItem item)
    {
        inventoryItems.Add(item);
        UpdateInventory();
    }

    public void Remove(string name)
    {
        var item = inventoryItems.FirstOrDefault(x => x.itemName == name);
        if (item == null)
        {
            OnItemNotForund?.Invoke();
            Debug.LogWarning($"Item with name \"{name}\" not found in inventory");
        }
        else
        {
            inventoryItems.Remove(item);
        }
        UpdateInventory();
    }

    public bool ItemExists(string name)
    {
        return inventoryItems.Any(x => x.itemName == name);
    }

    private void UpdateInventory()
    {
        foreach (Transform item in itemPlace)
        {
            Destroy(item.gameObject);
        }

        foreach(var item in inventoryItems)
        {
            var createdItem = Instantiate(itemPrefab, itemPlace);
            createdItem.sprite = item.sprite;
        }
    }
}
