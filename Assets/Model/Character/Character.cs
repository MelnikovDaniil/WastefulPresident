using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Character : Creature, ICharacterVisitor
{
    public Action OnMovementStart;
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

    private void FixedUpdate()
    {
        if (characterState != CharacterState.Dead)
        {
            if (isGrounded)
            {
                currentSpeed = speed;
                _rigidbody.sharedMaterial = fullFriction;
                if (disableTime <= 0)
                {
                    CalculateTargetMovement();
                }
                else
                {
                    disableTime -= Time.deltaTime;
                }
            }
            else
            {
                _animator.SetBool("walk", false);
            }

            if (!inFrontOfWall)
            {
                Move();
            }

            CheckFalling();
            CheckGround();
        }
    }

    public override void WalkTo(Vector2 position)
    {
        OnMovementStart?.Invoke();
        base.WalkTo(position);
    }

    public override void SetTarget(Vector2 target)
    {
        OnMovementStart?.Invoke();
        base.SetTarget(target);
    }

    protected void CheckPositionChanges()
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
                _animator.SetBool("walk", false);
                currentPositionTime = 0;
            }
        }
    }

    public abstract void SetColor(Color color);

    public abstract void ShowColor();

    public abstract void HideColor();

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
        SoundManager.PlaySound("ElectricPanelDeath");
        _animator.SetTrigger("electricity");
        Death();
    }

    public virtual void VisitPit(Action onPitFalling = null)
    {
    }

    public virtual void FinishVisitPit(Action onPitFalling = null)
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

    protected void CalculateTargetMovement()
    {
        if (target != null)
        {
            _rigidbody.sharedMaterial = zeroFriction;
            movementSide = Mathf.Sign(target.Value.x - transform.position.x);
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x) * movementSide * reversedSide,
                transform.localScale.y, 0);
            var targetDistanceX = Mathf.Abs(transform.position.x - target.Value.x);
            var targetDistanceY = Mathf.Abs(transform.position.y - target.Value.y);

            _animator.SetBool("walk", true);

            if (targetDistanceX < targetStopDistanceX
                && targetDistanceY < targetStopDistanceY)
            {
                HideTarget();
                if (characterState == CharacterState.MovingToInteract)
                {
                    characterState = CharacterState.Waiting;
                    TryInteract();
                }
                else
                {
                    characterState = CharacterState.Waiting;
                }
                _animator.SetBool("walk", false);
            }
            else if (targetDistanceX < targetStopDistanceX)
            {
                currentSpeed = 0;
            }
            CheckWall();
            CheckPositionChanges();
        }
    }

    protected IEnumerator QuestionMarkRoutine()
    {
        quesinMark.transform.parent = null;
        quesinMark.SetActive(true);
        quesinMark.transform.localScale = Vector3.one;
        quesinMark.transform.position = transform.position + Vector3.up * 3f;
        yield return new WaitForSeconds(1f);
        quesinMark.SetActive(false);
    }

    protected void TryInteract()
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
}
