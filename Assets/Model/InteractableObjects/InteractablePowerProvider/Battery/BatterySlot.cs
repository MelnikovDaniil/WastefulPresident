using UnityEngine;

public class BatterySlot : InteractrablePowerProvider
{
    public Transform batteryTransform;
    public GameObject light;

    private Battery storedBattery;
    public override void StartInteraction(IVisitor visitor)
    {
        var battery = visitor.GetBattery();
        if ((storedBattery == null && battery != null)
            || storedBattery != null)
        {
            visitor.StartTakingBattery(battery);
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

    public override void SuccessInteraction(IVisitor visitor)
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
