using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Agent : Human
{
    public float presidentStopDistance = 0.1f;

    public SpriteRenderer spriteRenderer;
    public SpriteRenderer skinRenderer;

    public new void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (humanState != HumanState.Dead)
        {
            if (isGrounded)
            {
                if (target != null)
                {
                    movementSide = Mathf.Sign(target.Value.x - transform.position.x);
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * movementSide, transform.localScale.y, 0);
                    var targetDistanceX = Mathf.Abs(transform.position.x - target.Value.x);
                    var targetDistanceY = Mathf.Abs(transform.position.y - target.Value.y);

                    if (humanState == HumanState.Follow)
                    {
                        _animator.SetBool("run", false);
                        _animator.SetBool("walk", true);
                    }
                    else
                    {
                        _animator.SetBool("run", true);
                        _animator.SetBool("walk", false);
                    }

                    if (humanState == HumanState.Follow && targetDistanceX < presidentStopDistance)
                    {
                        HideTarget();
                        _animator.SetBool("walk", false);
                    }
                    else if (targetDistanceX < targetStopDistanceX
                        && targetDistanceY < targetStopDistanceY)
                    {
                        if (humanState == HumanState.MovingToInteract)
                        {
                            humanState = HumanState.Waiting;
                            TryInteract();
                        }
                        else
                        {
                            humanState = HumanState.Waiting;
                        }
                        HideTarget();
                        _animator.SetBool("run", false);
                    }
                    CheckWall();
                    CheckPositionChanges();
                }
            }
            else
            {
                _animator.SetBool("walk", false);
                movementSide = previosSide; //* 2 / 3;
            }

            if (!inFrontOfWall)
            {
                _rigidbody.velocity = new Vector2(movementSide * speed, _rigidbody.velocity.y);
            }

            if (_rigidbody.velocity.y < -5f)
            {
                _animator.SetBool("fall", true);
            }
            else
            {
                _animator.SetBool("fall", false);
            }
            CheckGround();
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
}
