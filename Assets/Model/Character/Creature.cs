using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public Action OnDeath;
    public Action OnMovementFinish;
    public Action<IEnumerable<Collider2D>> OnLanding;
    public float speed;
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
    protected Vector2 slopeVectorPerp;

    protected float disableTime;

    private int groundMask = (1 << 6) | (1 << 7) | (1 << 8);

    protected void Awake()
    {
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

    public void WalkTo(Vector2 position)
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

    public void HideTarget()
    {
        OnMovementFinish?.Invoke();
        _rigidbody.velocity = Vector2.zero;
        movementSide = 0;
        target = null;
    }

    public void Disable(float time)
    {
        disableTime = time;
    }

    protected bool IsOnSlope()
    {
        var hit = Physics2D.Raycast(transform.position, Vector2.down, 2.5f, groundMask);

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
        var colliders = Physics2D.OverlapCircleAll(position, checkFroundRadius, groundMask);
        var anyCollider = colliders.Any(x => !x.isTrigger);
        if (isGrounded != anyCollider)
        {
            isGrounded = anyCollider;

            if (isGrounded)
            {
                //_rigidbody.velocity = Vector2.zero;
                OnLanding?.Invoke(colliders);
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

    protected void CheckFalling()
    {
        var isFalling = _rigidbody.velocity.y < -5f;
        _animator.SetBool("fall", isFalling);
    }

    protected void CheckWall()
    {
        var position = new Vector2(
            transform.position.x + checkWallOffset.x * Mathf.Sign(transform.localScale.x) * (reversed ? -1 : 1),
            transform.position.y + checkWallOffset.y);
        var colliders = Physics2D.OverlapCircleAll(position, checkFroundRadius, groundMask);

        inFrontOfWall = colliders.Any();
    }

    protected void OnDrawGizmos()
    {
        var groundPosition = new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundPosition, checkFroundRadius);

        var wallPosition = new Vector2(
            transform.position.x + checkWallOffset.x * Mathf.Sign(transform.localScale.x) * (reversed ? -1 : 1),
            transform.position.y + checkWallOffset.y);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(wallPosition, checkFroundRadius);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (characterState != CharacterState.Dead && collision.tag == "Bomb")
        {
            _animator.SetTrigger("bomb");
            Death();
        }
        else if (characterState != CharacterState.Dead && collision.tag == "DeathCollider")
        {
            _animator.SetTrigger("bomb");
            Death();
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
