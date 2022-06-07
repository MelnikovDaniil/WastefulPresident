using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Character : Creature, ICharacterVisitor
{
    [Space]
    public SpriteRenderer characterColor;
    public GameObject quesinMark;

    [NonSerialized]
    public Sprite icon;

    protected new void Awake()
    {
        base.Awake();
        characterColor.gameObject.SetActive(false);
    }

    protected void CheckPositionChanges()
    {
        if (isGrounded)
        {
            if (transform.position.x >= previosPositionX + samePositionDistance
                || transform.position.x <= previosPositionX - samePositionDistance)
            {
                previosPositionX = transform.position.x;
                currentPositionTime = 0;
            }
            else
            {
                currentPositionTime += Time.fixedDeltaTime;
                if (currentPositionTime >= samePositionTime)
                {
                    StartCoroutine(QuestionMarkRoutine());
                    HideTarget();
                    _animator.SetBool("run", false);
                    _animator.SetBool("walk", false);
                    currentPositionTime = 0;
                }
            }
        }
    }

    public abstract void SetColor(Color color);

    public abstract void ShowColor();

    public abstract void HideColor();

    public virtual void TryInteract()
    {
        var interactableObjects = Physics2D.OverlapCircleAll(transform.position, interactRadius)
            .Where(x => x.GetComponent<InteractableObject>());

        var collider = interactableObjects
            .OrderBy(x => Vector2.Distance(x.transform.position, transform.position))
            .FirstOrDefault();

        if (collider != null)
        {
            var interactableObject = collider.GetComponent<InteractableObject>();
            characterState = CharacterState.Acivating;
            interactableObject.StartInteraction(this);
        }
    }

    protected IEnumerator QuestionMarkRoutine()
    {
        quesinMark.SetActive(true);
        yield return new WaitForSeconds(1f);
        quesinMark.SetActive(false);
    }

    public void VisitLever()
    {
        _animator.SetTrigger("lever");
    }

    public void FinishVisiting()
    {
        characterState = CharacterState.Waiting;
    }

    public void VisitElectricPanel()
    {
        _animator.SetTrigger("electricPanel");
    }

    public void ElectricPanelDeath()
    {
        _animator.SetTrigger("electricity");
        Death();
    }

    public virtual void VisitPit()
    {
    }

    public virtual void FinishVisitPit()
    {
    }

    public void VisitTimer(float animationSpeed)
    {
        _animator.SetTrigger("timerOn");
        _animator.SetFloat("timerSpeed", animationSpeed);
    }

    public void FinishVisitTimer()
    {
        _animator.SetTrigger("timerOff");
    }

    public virtual Battery GetBattery()
    {
        return null;
    }

    public virtual void StartTakingBattery(Battery battery)
    {
        _animator.SetTrigger("batteryTake");
    }

    public virtual void PutBattery()
    {
    }

    public virtual bool TryTakeBattery(Battery battery)
    {
        return false;
    }

    public virtual void RemoveBattery()
    {
    }
}
