using UnityEngine;

[CreateAssetMenu(menuName = "InventoryItem", order = 1)]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public Sprite sprite;
}
