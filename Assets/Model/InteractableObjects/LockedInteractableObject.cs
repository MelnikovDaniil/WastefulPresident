using UnityEngine.Events;

public class LockedInteractableObject : InteractableObject
{
    public string unlockItemName;
    public UnityEvent unlockAction;
    public UnityEvent lockAction;

    public override void Interect()
    {
        if (InventoryManager.Instance.ItemExists(unlockItemName))
        {
            unlockAction?.Invoke();
            InventoryManager.Instance.Remove(unlockItemName);
        }
        else
        {
            lockAction?.Invoke();
        }
    }
}
