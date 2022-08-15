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
    public Vector2 timeRangeBeforeIdleSound = new Vector2(10, 20);

    public LayerMask searchingMask;
    public LayerMask targetMask;
    private Collider2D attackTransform;

    private float currentTimeBeforeIdleSound;
    private SMSound walkSound;
    private void Start()
    {
        currentTimeBeforeIdleSound = 5;
        walkSound = SoundManager.PlaySound("ZombieWalk")
            .SetLooped()
            .SetVolume(0);
        var attack = new GameObject("attack");
        var attackCollider = attack.AddComponent<CircleCollider2D>();
        attack.tag = "ZombieAttack";
        attackTransform = attackCollider;
        attack.SetActive(false);
    }

    private void FixedUpdate()
    {
        walkSound.SetVolume(0);
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

            CheckViores();
            CheckFalling();
            CheckGround();
        }
    }


    private void CalculateTargetMovement()
    {
        SearchTarget();
        if (target != null)
        {
            walkSound.SetVolume(1);
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
            CheckWall();
            CheckPositionChanges();
        }
        else
        {
            currentTimeBeforeIdleSound -= Time.deltaTime;
            if (currentTimeBeforeIdleSound <= 0)
            {
                currentTimeBeforeIdleSound = Random.Range(timeRangeBeforeIdleSound.x, timeRangeBeforeIdleSound.y);
                var animationNumber = Random.Range(1, 4);
                SoundManager.PlaySound("ZombieIdle0" + animationNumber);
            }
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

        if (hit != null && hit.TryGetComponent(out Creature creature) 
            && creature.characterState != CharacterState.Dead)
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
        SoundManager.PlaySound("ZombieAttack");
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
            WalkTo(hit.point + currentDirection * 1f);
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
