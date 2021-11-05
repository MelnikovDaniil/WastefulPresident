using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : Human
{
    private float horizontalMove = 0;

    public void Update()
    {
        if (!isDead)
        {
            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                isGrounded = false;
                _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }

            if (isGrounded && Input.GetKeyDown(KeyCode.W))
            {
                TryInteract();
            }

            horizontalMove = Input.GetAxisRaw("Horizontal") * speed;
        }
    }

    public void FixedUpdate()
    {
        if (!isDead)
        {
            _rigidbody.velocity = new Vector2(horizontalMove, _rigidbody.velocity.y);
            CheckGround();
        }
    }
}
