using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Agent : Human
{
    public float presidentStopDistance = 0.1f;

    private void FixedUpdate()
    {
        if (humanState != HumanState.Dead)
        {
            if (target != null)
            {
                var side = Mathf.Sign(target.Value.x - transform.position.x);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * side, transform.localScale.y, 0);
                _rigidbody.velocity = new Vector2(side * speed, _rigidbody.velocity.y);
                var targetDistance = Mathf.Abs(transform.position.x - target.Value.x);

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

                if (humanState == HumanState.Follow && targetDistance < presidentStopDistance)
                {
                    target = null;
                    _rigidbody.velocity = Vector2.zero;
                    _animator.SetBool("walk", false);
                }
                else if (targetDistance < targetStopDistance)
                {
                    target = null;
                    _rigidbody.velocity = Vector2.zero;
                    _animator.SetBool("run", false);
                    TryInteract();
                }

                CheckWall();
            }


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


    public void FollowPresedent(Vector2 target)
    {
        this.target = target;
    }
}
