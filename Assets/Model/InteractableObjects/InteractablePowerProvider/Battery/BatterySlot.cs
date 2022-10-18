using UnityEngine;

public class BatterySlot : InteractrablePowerProvider
{
    public Transform batteryTransform;
    public GameObject light;
    public Battery batteryPrefab;

    private Battery storedBattery;

    private void Start()
    {
        base.Start();
        if (isActive)
        {
            storedBattery = Instantiate(batteryPrefab, batteryTransform.transform);
            storedBattery.Hold();
        }
    }

    public override void StartInteraction(ICharacterVisitor visitor)
    {
        var item = visitor.GetItem();
        if ((storedBattery == null && item is Battery)
            || storedBattery != null)
        {
            if (storedBattery != null)
            {
                visitor.StartTakingItem(storedBattery);
                if (storedBattery.spriteRenderer.enabled == false)
                {
                    SoundManager.PlaySoundWithDelay("BatteryOutput", 0.2f);
                }
            }
            else
            {
                SoundManager.PlaySoundWithDelay("BatteryInput", 0.8f);
                visitor.PutItem();
            }
            base.StartInteraction(visitor);
        }
        else
        {
            visitor.FinishVisiting();
        }
    }

    public override void UpdateState()
    {
        light.SetActive(isActive);
    }

    public override void SuccessInteraction(ICharacterVisitor visitor)
    {
        var item = visitor.GetItem() as Battery;
        if (!isActive)
        {
            storedBattery = item;
            storedBattery.transform.parent = batteryTransform;
            storedBattery.transform.localPosition = Vector3.zero;
            storedBattery.transform.localRotation = Quaternion.identity;
            visitor.RemoveItem();
            storedBattery.Hold();
            base.SuccessInteraction(visitor);
        }
        else if (visitor.TryTakeItem(storedBattery))
        {
            base.SuccessInteraction(visitor);
            storedBattery = null;
        }
    }
}
