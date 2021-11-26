using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PressurePlate : InteractrablePowerProvider
{
    public Vector3 pressurePlacePosition;
    public Vector2 pressureSize;

    public SpriteRenderer sprteRenderer;
    public Sprite pressedSprite;
    public Sprite releasedSprite;

    public float pressureCheckPeriod = 0.5f;

    public new void Start()
    {
        base.Start();
        StartCoroutine(CheckPressureRoutine());
    }

    public override void SuccessInteraction(IVisitor visitor)
    {
    }

    public override void UpdateState()
    {
        if (isActive)
        {
            sprteRenderer.sprite = pressedSprite;
        }
        else
        {
            sprteRenderer.sprite = releasedSprite;
        }
    }

    private IEnumerator CheckPressureRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(pressureCheckPeriod);
            var colliers = Physics2D.OverlapBoxAll(pressurePlacePosition + transform.position, pressureSize, 0);
            var people = colliers.Where(x => x.GetComponent<Human>() && x.GetComponent<Human>().humanState != HumanState.Dead);
            if (!isActive && people.Any())
            {
                isActive = true;
                TurnEnergy();
                UpdateState();
            }
            else if (isActive && !people.Any())
            {
                isActive = false;
                TurnEnergy();
                UpdateState();
            }
        }
    }

    protected new void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(pressurePlacePosition + transform.position, pressureSize);
    }
}
