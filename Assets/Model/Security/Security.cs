using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Security : Human
{
    public float targetStopDistance = 0.1f;
    public float presidentStopDistance = 0.1f;


    [Space]
    public Vector2 checkWallOffset = new Vector2(1.2f, 0.5f);

    [NonSerialized]
    public Vector2? target;

    private bool inFrontOfWall;
    private bool jumpDelay;
    private bool isFollowPresedent;

    private void Update()
    {
        if (!isDead && !jumpDelay && inFrontOfWall && isGrounded)
        {
            isGrounded = false;
            jumpDelay = true;
            _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            _animator.SetTrigger("jump");
            StartCoroutine(JumpDelayRoutine());
        }
    }

    private IEnumerator JumpDelayRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        jumpDelay = false;
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            if (target != null)
            {
                var side = Mathf.Sign(target.Value.x - transform.position.x);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * side, transform.localScale.y, 0);
                _rigidbody.velocity = new Vector2(side * speed, _rigidbody.velocity.y);
                var targetDistance = Mathf.Abs(transform.position.x - target.Value.x);
                _animator.SetBool("run", true);
                if (isFollowPresedent && targetDistance < presidentStopDistance)
                {
                    isFollowPresedent = false;
                    target = null;
                    _rigidbody.velocity = Vector2.zero;
                    _animator.SetBool("run", false);
                }
                if (targetDistance < targetStopDistance)
                {
                    target = null;
                    _rigidbody.velocity = Vector2.zero;
                    TryInteract();
                    _animator.SetBool("run", false);
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
        isFollowPresedent = true;
        this.target = target;
    }

    public override void Death()
    {
        base.Death();
        target = null;
    }

    private void CheckWall()
    {
        var position = new Vector2(
            transform.position.x + checkWallOffset.x * Mathf.Sign(transform.localScale.x),
            transform.position.y + checkWallOffset.y);
        var colliders = Physics2D.OverlapCircleAll(position, checkFroundRadius).Where(x => x.gameObject.layer == 6);

        inFrontOfWall = colliders.Any();
    }

    private void OnDrawGizmos()
    {
        var groundPosition = new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundPosition, checkFroundRadius);

        var wallPosition = new Vector2(
            transform.position.x + checkWallOffset.x * Mathf.Sign(transform.localScale.x),
            transform.position.y + checkWallOffset.y);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(wallPosition, checkFroundRadius);
    }
}
