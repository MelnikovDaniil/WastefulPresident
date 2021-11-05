using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : Human
{
    [Space]
    public Vector2 cameraOffset = Vector2.up;

    private float horizontalMove = 0;

    private void Start()
    {
        CameraManager.Instance.SetTarget(gameObject, 5, -1, cameraOffset);
    }

    public void Update()
    {
        if (!isDead)
        {
            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                isGrounded = false;
                _animator.SetTrigger("jump");
                _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }

            if (isGrounded && Input.GetKeyDown(KeyCode.W))
            {
                TryInteract();
            }

            horizontalMove = Input.GetAxisRaw("Horizontal") * speed;
            if (Mathf.Abs(horizontalMove) > 0)
            {
                _animator.SetBool("walk", true);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Input.GetAxisRaw("Horizontal"), transform.localScale.y, 0);
            }
            else
            {
                _animator.SetBool("walk", false);
            }
        }
    }

    public void FixedUpdate()
    {
        if (!isDead)
        {
            _rigidbody.velocity = new Vector2(horizontalMove, _rigidbody.velocity.y);
            if (_rigidbody.velocity.y < -0.1)
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

    public void SendOrder()
    {
        _animator.SetTrigger("order");
    }

    public new void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.TryGetComponent<DialogueTrigger>(out var dialogueTrigger))
        {
            dialogueTrigger.TriggerDialogue();
        }
    }
}
