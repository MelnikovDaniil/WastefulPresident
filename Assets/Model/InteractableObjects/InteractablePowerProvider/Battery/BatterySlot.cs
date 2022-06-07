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
        var battery = visitor.GetBattery();
        if ((storedBattery == null && battery != null)
            || storedBattery != null)
        {
            if (storedBattery != null)
            {
                visitor.StartTakingBattery(storedBattery);
            }
            else
            {
                visitor.PutBattery();
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
        var battery = visitor.GetBattery();
        if (!isActive)
        {
            storedBattery = battery;
            storedBattery.transform.parent = batteryTransform;
            storedBattery.transform.localPosition = Vector3.zero;
            storedBattery.transform.localRotation = Quaternion.identity;
            visitor.RemoveBattery();
            storedBattery.Hold();
            base.SuccessInteraction(visitor);
        }
        else if (visitor.TryTakeBattery(storedBattery))
        {
            base.SuccessInteraction(visitor);
            storedBattery = null;
        }
    }
}
