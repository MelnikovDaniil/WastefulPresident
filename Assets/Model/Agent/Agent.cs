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

    private void FixedUpdate()
    {
        if (characterState != CharacterState.Dead)
        {
            if (isGrounded)
            {
                _rigidbody.sharedMaterial = fullFriction;
                if (disableTime <= 0)
                {
                    if (target != null)
                    {
                        _rigidbody.sharedMaterial = zeroFriction;
                        movementSide = Mathf.Sign(target.Value.x - transform.position.x);
                        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * movementSide * (reversed ? -1 : 1), transform.localScale.y, 0);
                        var targetDistanceX = Mathf.Abs(transform.position.x - target.Value.x);
                        var targetDistanceY = Mathf.Abs(transform.position.y - target.Value.y);

                        if (characterState == CharacterState.Follow)
                        {
                            _animator.SetBool("run", false);
                            _animator.SetBool("walk", true);
                        }
                        else
                        {
                            _animator.SetBool("run", true);
                            _animator.SetBool("walk", false);
                        }

                        if (characterState == CharacterState.Follow && targetDistanceX < presidentStopDistance)
                        {
                            HideTarget();
                            _animator.SetBool("walk", false);
                        }
                        else if (targetDistanceX < targetStopDistanceX
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
                            _animator.SetBool("run", false);
                        }
                        CheckWall();
                        CheckPositionChanges();
                    }
                }
                else
                {
                    disableTime -= Time.deltaTime;
                }
            }
            else
            {
                _animator.SetBool("run", false);
                _animator.SetBool("walk", false);
            }

            if (!inFrontOfWall)
            {
                if (IsOnSlope())
                {
                    _rigidbody.velocity = movementSide * speed * -slopeVectorPerp;
                }
                else
                {
                    _rigidbody.velocity = new Vector2(movementSide * speed, _rigidbody.velocity.y);

                }
            }

            CheckFalling();
            CheckGround();
        }
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
