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
    public float attackDelay = 0.2f;

    public LayerMask searchingMask;
    public LayerMask targetMask;
    private Collider2D attackTransform;

    private void Start()
    {
        var attack = new GameObject("attack");
        var attackCollider = attack.AddComponent<CircleCollider2D>();
        attack.tag = "ZombieAttack";
        attackTransform = attackCollider;
    }

    private void FixedUpdate()
    {
        if (characterState != CharacterState.Dead)
        {
            if (isGrounded)
            {
                currentSpeed = speed;
                _rigidbody.sharedMaterial = fullFriction;
                if (disableTime <= 0)
                {
                    CalculateTargetMovement();
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
                Move();
            }

            CheckFalling();
            CheckGround();
        }
    }


    private void CalculateTargetMovement()
    {
        SearchTarget();
        if (target != null)
        {
            _rigidbody.sharedMaterial = zeroFriction;
            movementSide = Mathf.Sign(target.Value.x - transform.position.x);
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * movementSide * reversedSide, transform.localScale.y, 0);
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
            CheckPositionChanges();
        }
    }

    protected void CheckPositionChanges()
    {
        if (isGrounded)
        {
            if (transform.position.x >= previosPositionX + samePositionDistance
                || transform.position.x <= previosPositionX - samePositionDistance)
            {
                previosPositionX = transform.position.x;
                currentPositionTime = 0;
            }
            else
            {
                currentPositionTime += Time.fixedDeltaTime;
                if (currentPositionTime >= samePositionTime)
                {
                    HideTarget();
                    _animator.SetBool("run", false);
                    _animator.SetBool("walk", false);
                    currentPositionTime = 0;
                }
            }
        }
    }

    private void CheckViores()
    {
        var attackPosition = transform.position + Vector3.right * attackDistance * Mathf.Sign(transform.localScale.x) * reversedSide;
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
        var attackPosition = transform.position + Vector3.right * attackDistance * Mathf.Sign(transform.localScale.x) * reversedSide;
        yield return new WaitForSeconds(attackDelay);
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
        var currentDirection = Vector2.right* Mathf.Sign(transform.localScale.x) * reversedSide;
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
            WalkTo(hit.point + currentDirection * 0.7f);
        }
    }

    private new void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.right * 20);
        Gizmos.DrawRay(transform.position, Vector2.left * 20);

        Gizmos.DrawWireSphere(
            transform.position + Vector3.right * attackDistance * Mathf.Sign(transform.localScale.x) * reversedSide,
            attackRadius);
    }
}
