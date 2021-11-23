using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricPanel : InteractrablePowerProvider
{
    public SpriteRenderer spriteRenderer;
    public Sprite brokenPanelSprite;

    private bool isBroken;

    public override void StartInteraction(IVisitor visitor)
    {
        if (!isBroken)
        {
            base.StartInteraction(visitor);
            visitor.VisitElectricPanel();
        }
    }

    public override void SuccessInteraction(IVisitor visitor)
    {
        if (!isBroken)
        {
            isBroken = true;
            base.SuccessInteraction(visitor);
            visitor.ElectricPanelDeath();
            GetComponent<BoxCollider2D>().enabled = false;
            StopAllCoroutines();
        }
    }

    public override void UpdateState()
    {
        if (isActive)
        {
            spriteRenderer.sprite = brokenPanelSprite;
        }
    }
}
