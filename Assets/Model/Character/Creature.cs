using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Creature : MonoBehaviour, IPortalVisitor
{
    public Action OnDeath;
    public Action OnMovementFinish;
    public Action<IEnumerable<Collider2D>> OnLanding;
    public float speed;
    public float fallingXSpeed = 3.5f;
    public float jumpForce;
    public bool reversed = false;

    [Space]
    public float targetStopDistanceX = 0.1f;
    public float targetStopDistanceY = 1f;
    public float checkGroundOffsetY = -1.8f;
    public float checkFroundRadius = 0.3f;
    public float interactRadius = 0.5f;
    public Vector2 checkWallOffset = new Vector2(1.2f, 0.5f);

    [Space]
    public PhysicsMaterial2D fullFriction;
    public PhysicsMaterial2D zeroFriction;

    [Space]
    public float samePositionTime = 0.1f;
    public float samePositionDistance = 0.05f;

    [NonSerialized]
    public CharacterState characterState;



    protected Rigidbody2D _rigidbody;
    protected Animator _animator;

    protected Vector2? target;

    protected float currentPositionTime;
    protected float previosPositionX;

    protected bool isGrounded;

    protected bool inFrontOfWall;
    protected bool jumpDelay;

    protected float movementSide;
    protected float currentSpeed;
    protected Vector2 slopeVectorPerp;

    protected float disableTime;
    protected float reversedSide;

    private LayerMask _groundMask;

    protected void Awake()
    {
        reversedSide = reversed ? -1 : 1;
        _groundMask = LayerMask.GetMask("Ground", "Door", "Trampoline");
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    public virtual void Death()
    {
        characterState = CharacterState.Dead;
        OnDeath?.Invoke();
        _rigidbody.sharedMaterial = null;
        target = null;
    }

    public virtual void WalkTo(Vector2 position)
    {
        if (characterState != CharacterState.Dead
            && characterState != CharacterState.Acivating)
        {
            currentPositionTime = 0;
            characterState = CharacterState.Walking;
            this.target = position;
        }
    }

    public virtual void SetTarget(Vector2 target)
    {
        if (characterState != CharacterState.Dead
            && characterState != CharacterState.Acivating)
        {
            currentPositionTime = 0;
            characterState = CharacterState.MovingToInteract;
            this.target = target;
        }
    }

    public Vector2? GetTarget()
    {
        return target;
    }

    public void HideTarget()
    {
        OnMovementFinish?.Invoke();
        //_rigidbody.velocity = Vector2.zero;
        movementSide = 0;
        target = null;
    }

    public void Disable(float time)
    {
        disableTime = time;
    }

    protected bool IsOnSlope()
    {
        var hit = Physics2D.Raycast(transform.position, Vector2.down, 2.5f, _groundMask);

        if (isGrounded && hit && hit.normal != Vector2.up)
        {
            slopeVectorPerp = Vector2.Perpendicular(hit.normal).normalized;
            return true;
        }

        return false;
    }

    protected void CheckGround()
    {
        var position = new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY);
        var colliders = Physics2D.OverlapCircleAll(position, checkFroundRadius, _groundMask);
        var anyCollider = colliders.Any(x => !x.isTrigger);
        if (isGrounded != anyCollider)
        {
            isGrounded = anyCollider;

            if (isGrounded)
            {
                disableTime = 0.5f;
                OnLanding?.Invoke(colliders);
            }
            else
            {
                _rigidbody.sharedMaterial = zeroFriction;
                currentSpeed = fallingXSpeed;
            }
            _animator.SetBool("grounded", isGrounded);
        }
    }

    public void CheckTrampoline()
    {
        if (movementSide == 0 && target.HasValue)
        {
            movementSide = Mathf.Sign(target.Value.x - transform.position.x);
        }
        isGrounded = false;
    }

    public void Teleport(Vector3 position, Vector3 direction)
    {
        movementSide = direction.x;
        WalkTo(position + direction);
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(direction.x) * reversedSide,
            transform.localScale.y,
            transform.localScale.z);
        transform.position = position;
    }

    protected void Move()
    {
        if (IsOnSlope())
        {
            _rigidbody.velocity = movementSide * currentSpeed * -slopeVectorPerp;
        }
        else
        {
            _rigidbody.velocity = new Vector2(movementSide * currentSpeed, _rigidbody.velocity.y);

        }
    }

    protected void CheckFalling()
    {
        var maxFallVelocity = -20f;

        if (_rigidbody.velocity.y < maxFallVelocity)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, maxFallVelocity);
        }
    }

    protected void CheckWall()
    {
        var position = new Vector2(
            transform.position.x + checkWallOffset.x * Mathf.Sign(transform.localScale.x) * reversedSide,
            transform.position.y + checkWallOffset.y);
        var colliders = Physics2D.OverlapCircleAll(position, checkFroundRadius, _groundMask);

        inFrontOfWall = colliders.Any();
    }

    protected void OnDrawGizmos()
    {
        reversedSide = reversed ? -1 : 1;
        var groundPosition = new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundPosition, checkFroundRadius);

        var wallPosition = new Vector2(
            transform.position.x + checkWallOffset.x * Mathf.Sign(transform.localScale.x) * reversedSide,
            transform.position.y + checkWallOffset.y);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(wallPosition, checkFroundRadius);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (characterState != CharacterState.Dead)
        {
            switch (collision.tag)
            {
                case "Bomb":
                    _animator.SetTrigger("bomb");
                    Death();
                    break;
                case "DeathCollider":
                    _animator.SetTrigger("bomb");
                    Death();
                    break;
                case "Gas":
                    _animator.SetTrigger("bomb");
                    Death();
                    break;
                case "ZombieAttack":
                    _animator.SetTrigger("deathByZombie");
                    Death();
                    break;
            }
        }
            
    }

    private void OnParticleCollision(GameObject other)
    {
        if (characterState != CharacterState.Dead && other.tag == "DeathCollider")
        {
            _animator.SetTrigger("bomb");
            Death();
        }
    }
}
