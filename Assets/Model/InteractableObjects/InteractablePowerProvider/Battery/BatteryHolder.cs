using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryHolder : InteractableObject
{
    public Battery batteryPrefab;
    public GameObject batteryTransform;
    public Animator _holderAnimator;
    public GameObject light;

    private Battery battery;

    private void Start()
    {
        foreach (Transform child in batteryTransform.transform)
        {
            Destroy(child.gameObject);
        }
        battery = Instantiate(batteryPrefab, batteryTransform.transform);
        battery.Hold();
    }

    public override void StartInteraction(ICharacterVisitor visitor)
    {
        if (battery != null)
        {
            visitor.StartTakingItem(battery);
            base.StartInteraction(visitor);
        }
        else
        {
            visitor.FinishVisiting();
        }
    }
    public override void SuccessInteraction(ICharacterVisitor visitor)
    {
        if (visitor.TryTakeItem(battery))
        {
            light.SetActive(false);
            GetComponent<Collider2D>().enabled = false;
            battery = null;
            batteryTransform.SetActive(false);
        }
    }
}
