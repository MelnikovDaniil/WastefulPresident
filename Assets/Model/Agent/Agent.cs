using System;
using UnityEngine;

public class Agent : Character
{
    public float presidentStopDistance = 0.1f;

    public SpriteRenderer spriteRenderer;
    public SpriteRenderer skinRenderer;

    [Space]
    public Transform backTransform;

    public SpriteRenderer itemAnimationSprite;
    private Item currentItem;

    public new void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Death()
    {
        base.Death();
        if (currentItem != null)
        {
            currentItem.Throw();
            currentItem.transform.parent = null;
            currentItem = null;
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

    public override void VisitPit(Action onPitFalling = null)
    {
        OnMovementStart += onPitFalling;
        _animator.SetBool("pit", true);
        spriteRenderer.sortingOrder += 10;
        skinRenderer.sortingOrder += 10;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public override void FinishVisitPit(Action onPitFalling = null)
    {
        OnMovementStart -= onPitFalling;
        _animator.SetBool("pit", false);
        disableTime = 1;
        spriteRenderer.sortingOrder -= 10;
        skinRenderer.sortingOrder -= 10;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public override Item GetItem()
    {
        return currentItem;
    }

    public override void StartTakingItem(Item item)
    {
        item.spriteRenderer.enabled = false;
        itemAnimationSprite.sprite = item.spriteRenderer.sprite;
        base.StartTakingItem(item);
    }

    public override void PutItem()
    {
        currentItem.spriteRenderer.enabled = false;
        itemAnimationSprite.sprite = currentItem.spriteRenderer.sprite;
        _animator.SetTrigger("batteryPut");
        base.StartTakingItem(currentItem);
    }

    public override bool TryTakeItem(Item item)
    {
        item.spriteRenderer.enabled = true;
        if (currentItem != null)
        {
            currentItem.transform.parent = null;
            currentItem.Throw();
        }

        currentItem = item;
        currentItem.transform.parent = backTransform;
        currentItem.transform.localPosition = Vector2.zero;
        currentItem.transform.localRotation = Quaternion.identity;
        currentItem.Hold();

        return true;
    }

    public override void RemoveItem()
    {
        currentItem = null;
    }
}
