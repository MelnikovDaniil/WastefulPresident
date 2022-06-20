using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zombie : Creature
{
    [Space]
    public float attackDistance = 2;
    public float attackRadius = 1;
    public float attackRate = 1.5f;

    public LayerMask searchingMask;
    public LayerMask targetMask;
    private Collider2D attackTransform;

    private void Start()
    {
        var attack = new GameObject("attack");
        var attackCollider = attack.AddComponent<CircleCollider2D>();
        attack.tag = "DeathCollider";
        attackTransform = attackCollider;
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
                            characterState = CharacterState.Waiting;
                            _animator.SetBool("walk", false);
                        }
                        CheckViores();
                        CheckWall();
                        //CheckPositionChanges();
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
        var attackPosition = transform.position + Vector3.right * attackDistance * Mathf.Sign(transform.localScale.x) * (reversed ? -1 : 1);
        var hit = Physics2D.OverlapCircle(attackPosition, attackRadius, targetMask);

        if (hit != null)
        {
            Attack();
        }
    }

    private void Attack()
    {
        _animator.SetTrigger("attack");
        disableTime = attackRate;
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        var attackPosition = transform.position + Vector3.right * attackDistance * Mathf.Sign(transform.localScale.x) * (reversed ? -1 : 1);
        yield return new WaitForSeconds(attackRate);
        var hitedColliders = Physics2D.OverlapCircleAll(attackPosition, attackRadius + 1, targetMask);
        foreach (var collider in hitedColliders)
        {
            if (collider.gameObject != gameObject)
            {
                var character = collider.GetComponent<Character>();
                character.OnTriggerEnter2D(attackTransform);
            }
        }
    }

    private void SearchTarget()
    {
        var currentDirection = Vector2.right* Mathf.Sign(transform.localScale.x) * (reversed ? -1f : 1f);
        var raycasts = new List<RaycastHit2D>();
        var frontRaycastHit = Physics2D.Raycast(
            transform.position,
            currentDirection,
            200, searchingMask);
        raycasts.Add(frontRaycastHit);
        if (target == null)
        {
            var backRaycast = Physics2D.Raycast(
                transform.position,
                -currentDirection,
                200, searchingMask);
            raycasts.Add(backRaycast);
        }

        var hit = raycasts.FirstOrDefault(x => (targetMask & (1 << x.collider.gameObject.layer)) > 0);
        if (hit.collider != null)
        {
            WalkTo(hit.point + currentDirection);
        }
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