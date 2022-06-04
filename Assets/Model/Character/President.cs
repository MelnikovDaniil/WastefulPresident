using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class President : Character, IPresidentVisitor
{
    [NonSerialized]
    public bool isLocked;
    [Space]
    [Range(0f, 1f)]
    public float sendOrderChanse = 1;

    [Space]
    public Material colorMaterial;

    [Space]
    public float watchingClockFromTime = 15f;

    private float currentIdleTime = 0;

    public void Update()
    {
        if (!isLocked && !DialogueManager.isWorking && characterState != CharacterState.Dead)
        {
            if (characterState == CharacterState.Waiting)
            {
                currentIdleTime += Time.deltaTime;
                if (currentIdleTime >= watchingClockFromTime)
                {
                    WatchClock();
                }
            }
        }
    }

    public void FixedUpdate()
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
                        currentIdleTime = 0;
                        movementSide = Mathf.Sign(target.Value.x - transform.position.x);
                        transform.localScale = new Vector3(
                            Mathf.Abs(transform.localScale.x) * movementSide * (reversed ? -1 : 1),
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

    public override void SetColor(Color color)
    {
        colorMaterial.SetColor("_Color", color);
        colorMaterial.SetFloat("_Thickness", 0f);
    }

    public override void ShowColor()
    {
        colorMaterial.SetFloat("_Thickness", 1.5f);
    }

    public override void HideColor()
    {
        colorMaterial.SetFloat("_Thickness", 0f);
    }

    public void SendOrder()
    {
        if (sendOrderChanse > Random.value)
        {
            _animator.SetTrigger("order");
        }
    }

    public void PlayAnimation(string animName)
    {
        _animator.Play(animName);
    }

    public new void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.TryGetComponent<ColliderDialogueTrigger>(out var dialogueTrigger))
        {
            dialogueTrigger.TriggerDialogue();
        }
    }

    private void WatchClock()
    {
        _animator.SetTrigger("clock");
        currentIdleTime = 0;
    }
}
