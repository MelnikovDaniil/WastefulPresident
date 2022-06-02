using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zombie : Human
{
    [Space]
    public float attackDistance = 2;
    public float attackRadius = 1;
    public float attackRate = 1.5f;

    private LayerMask searchingMask;
    private bool isAttacking;

    private void Start()
    {
        searchingMask = LayerMask.GetMask("Characters");
    }

    private void FixedUpdate()
    {
        if (humanState != HumanState.Dead)
        {
            if (isGrounded)
            {
                _rigidbody.sharedMaterial = fullFriction;
                if (disableTime <= 0)
                {
                    SearchTarget();
                    if (target != null)
                    {
                        _rigidbody.sharedMaterial = zeroFriction;
                        movementSide = Mathf.Sign(target.Value.x - transform.position.x);
                        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * movementSide * (reversed ? -1 : 1), transform.localScale.y, 0);
                        var targetDistanceX = Mathf.Abs(transform.position.x - target.Value.x);
                        var targetDistanceY = Mathf.Abs(transform.position.y - target.Value.y);

                        _animator.SetBool("walk", true);
                        
                        if (targetDistanceX < targetStopDistanceX
                            && targetDistanceY < targetStopDistanceY)
                        {
                            HideTarget();
                            if (humanState == HumanState.MovingToInteract)
                            {
                                humanState = HumanState.Waiting;
                                TryInteract();
                            }
                            else
                            {
                                humanState = HumanState.Waiting;
                            }
                            _animator.SetBool("walk", false);
                        }
                        CheckViores();
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

    private void CheckViores()
    {
        //if (!isAttacking)
        //{
            var attackPosition = transform.position + Vector3.right * attackDistance * Mathf.Sign(transform.localScale.x) * (reversed ? -1 : 1);
            var hit = Physics2D.OverlapCircle(attackPosition, attackRadius, searchingMask);

            if (hit != null)
            {
                Attack();
            }
        //}
    }

    private void Attack()
    {
        //isAttacking = true;
        _animator.SetTrigger("electricPanel");
        disableTime = attackRate;
        //yield return new WaitForSeconds(attackRate);
    }

    //private IEnumerator AttackRouting()
    //{
    //    isAttacking = true;
    //    _animator.SetTrigger("electricPanel");
    //    yield return new WaitForSeconds(attackRate);
    //}

    private void SearchTarget()
    {
        var raycasts = new List<RaycastHit2D>();
        var frontRaycastHit = Physics2D.Raycast(
            transform.position,
            Vector2.right * Mathf.Sign(transform.localScale.x) * (reversed ? -1f : 1f),
            200, searchingMask);
        raycasts.Add(frontRaycastHit);
        if (target == null)
        {
            var backRaycast = Physics2D.Raycast(
                transform.position,
                Vector2.left * Mathf.Sign(transform.localScale.x) * (reversed ? -1f : 1f),
                200, searchingMask);
            raycasts.Add(backRaycast);
        }

        var hit = raycasts.FirstOrDefault(x => x.collider != null);
        if (hit.collider != null)
        {
            WalkTo(hit.point);
        }
    }

    public override void HideColor()
    {
        //throw new System.NotImplementedException();
    }

    public override void SetColor(Color color)
    {
        //throw new System.NotImplementedException();
    }

    public override void ShowColor()
    {
        //throw new System.NotImplementedException();
    }

    private new void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.right * 20);
        Gizmos.DrawRay(transform.position, Vector2.left * 20);

        Gizmos.DrawWireSphere(
            transform.position + Vector3.right * attackDistance * Mathf.Sign(transform.localScale.x) * (reversed ? -1 : 1),
            attackRadius);
    }
}
