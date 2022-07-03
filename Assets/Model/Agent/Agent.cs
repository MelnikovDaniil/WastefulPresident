using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Agent : Character
{
    public float presidentStopDistance = 0.1f;

    public SpriteRenderer spriteRenderer;
    public SpriteRenderer skinRenderer;

    [Space]
    public Transform backTransform;

    private Battery currentBattery;

    public new void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Death()
    {
        base.Death();
        if (currentBattery != null)
        {
            currentBattery.Throw();
            currentBattery.transform.parent = null;
            currentBattery = null;
        }
    }

    public override void SetColor(Color color)
    {
        spriteRenderer.material.SetColor("_Color", color);
        spriteRenderer.material.SetFloat("_Thickness", 0f);
    }

    public override void ShowColor()
    {
        spriteRenderer.material.SetFloat("_Thickness", 1.5f);
    }

    public override void HideColor()
    {
        spriteRenderer.material.SetFloat("_Thickness", 0f);
    }

    public void FollowPresedent(Vector2 target)
    {
        var targetDistanceX = Mathf.Abs(transform.position.x - target.x);
        if (targetDistanceX > presidentStopDistance)
        {
            currentPositionTime = 0;
            this.target = target;
        }
    }

    public override void VisitPit()
    {
        _animator.SetBool("pit", true);
        spriteRenderer.sortingOrder += 10;
        skinRenderer.sortingOrder += 10;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public override void FinishVisitPit()
    {
        _animator.SetBool("pit", false);
        disableTime = 1;
        spriteRenderer.sortingOrder -= 10;
        skinRenderer.sortingOrder -= 10;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public override Battery GetBattery()
    {
        return currentBattery;
    }

    public override void StartTakingBattery(Battery battery)
    {
        battery.spriteRenderer.enabled = false;
        base.StartTakingBattery(battery);
    }

    public override void PutBattery()
    {
        currentBattery.spriteRenderer.enabled = false;
        _animator.SetTrigger("batteryPut");
        base.StartTakingBattery(currentBattery);
    }

    public override bool TryTakeBattery(Battery battery)
    {
        battery.spriteRenderer.enabled = true;
        if (currentBattery == null)
        {
            currentBattery = battery;
            currentBattery.transform.parent = backTransform;
            currentBattery.transform.localPosition = Vector2.zero;
            currentBattery.transform.localRotation = Quaternion.identity;
            currentBattery.Hold();
        }
        else
        {
            battery.transform.parent = null;
            battery.Throw();
        }

        return true;
    }

    public override void RemoveBattery()
    {
        currentBattery = null;
    }
}
