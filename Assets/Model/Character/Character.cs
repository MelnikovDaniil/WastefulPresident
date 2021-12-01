using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : Human, ICharacterVisitor
{
    [NonSerialized]
    public bool isLocked;
    [Space]
    public Vector2 cameraOffset = Vector2.up;

    private float horizontalMove = 0;


    public void Update()
    {
        if (!isLocked && !DialogueManager.isWorking && humanState != HumanState.Dead)
        {
            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                isGrounded = false;
                _animator.SetTrigger("jump");
                _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }

            if (isGrounded && Input.GetKeyDown(KeyCode.E))
            {
                TryInteract();
            }

            horizontalMove = Input.GetAxisRaw("Horizontal") * speed;
            if (Mathf.Abs(horizontalMove) > 0)
            {
                _animator.SetBool("walk", true);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Input.GetAxisRaw("Horizontal"), transform.localScale.y, 0);
            }
            else if (humanState == HumanState.Waiting)
            {
                _animator.SetBool("walk", false);
            }
        }
        else
        {
            _animator.SetBool("walk", false);
            horizontalMove = 0;
        }
    }

    public void FixedUpdate()
    {
        if (humanState != HumanState.Dead)
        {
            if (target != null)
            {
                var side = Mathf.Sign(target.Value.x - transform.position.x);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * side, transform.localScale.y, 0);
                _rigidbody.velocity = new Vector2(side * speed, _rigidbody.velocity.y);
                var targetDistanceX = Mathf.Abs(transform.position.x - target.Value.x);
                var targetDistanceY = Mathf.Abs(transform.position.y - target.Value.y);

                _animator.SetBool("walk", true);

                if (targetDistanceX < targetStopDistanceX)
                {
                    if (humanState == HumanState.MovingToInteract)
                    {
                        TryInteract();
                    }
                    else
                    {
                        humanState = HumanState.Waiting;
                    }
                    target = null;
                    _rigidbody.velocity = Vector2.zero;
                    _animator.SetBool("walk", false);

                }
                CheckWall();
                CheckPositionChanges();
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

    public void WalkTo(Vector2 position)
    {
        humanState = HumanState.Walking;
        this.target = position;
    }

    public void SendOrder()
    {
        _animator.SetTrigger("order");
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
}
