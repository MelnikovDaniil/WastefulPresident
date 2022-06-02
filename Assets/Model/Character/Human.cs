using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Human : MonoBehaviour, IVisitor
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

    [Space]
    public SpriteRenderer characterColor;
    public GameObject quesinMark;

    [NonSerialized]
    public HumanState humanState;
    [NonSerialized]
    public Sprite icon;

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
        characterColor.gameObject.SetActive(false);
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    public abstract void SetColor(Color color);

    public abstract void ShowColor();

    public abstract void HideColor();

    protected IEnumerator JumpDelayRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        jumpDelay = false;
    }

    public virtual void TryInteract()
    {
        var interactableObjects = Physics2D.OverlapCircleAll(transform.position, interactRadius)
            .Where(x => x.GetComponent<InteractableObject>());

        var collider = interactableObjects
            .OrderBy(x => Vector2.Distance(x.transform.position, transform.position))
            .FirstOrDefault();

        if (collider != null)
        {
            var interactableObject = collider.GetComponent<InteractableObject>();
            humanState = HumanState.Acivating;
            interactableObject.StartInteraction(this);
        }
    }

    public virtual void Death()
    {
        humanState = HumanState.Dead;
        OnDeath?.Invoke();
        _rigidbody.sharedMaterial = null;
        target = null;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (humanState != HumanState.Dead && collision.tag == "Bomb")
        {
            _animator.SetTrigger("bomb");
            Death();
        }
        else if (humanState != HumanState.Dead && collision.tag == "DeathCollider")
        {
            _animator.SetTrigger("bomb");
            Death();
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (humanState != HumanState.Dead && other.tag == "DeathCollider")
        {
            _animator.SetTrigger("bomb");
            Death();
        }
    }

    public void WalkTo(Vector2 position)
    {
        if (humanState != HumanState.Dead
            && humanState != HumanState.Acivating)
        {
            currentPositionTime = 0;
            humanState = HumanState.Walking;
            this.target = position;
        }
    }

    public virtual void SetTarget(Vector2 target)
    {
        if (humanState != HumanState.Dead
            && humanState != HumanState.Acivating)
        {
            currentPositionTime = 0;
            humanState = HumanState.MovingToInteract;
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
                    StartCoroutine(QuestionMarkRoutine());
                    HideTarget();
                    _animator.SetBool("run", false);
                    _animator.SetBool("walk", false);
                    currentPositionTime = 0;
                }
            }
        }
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
        var maxFallVelocity = -20f;
        var isFalling = _rigidbody.velocity.y < -5f;
        _animator.SetBool("fall", isFalling);

        if (_rigidbody.velocity.y < maxFallVelocity)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, maxFallVelocity);
        }
    }

    protected void CheckWall()
    {
        var position = new Vector2(
            transform.position.x + checkWallOffset.x * Mathf.Sign(transform.localScale.x) * (reversed ? -1 : 1),
            transform.position.y + checkWallOffset.y);
        var colliders = Physics2D.OverlapCircleAll(position, checkFroundRadius, groundMask);

        inFrontOfWall = colliders.Any();
    }

    protected IEnumerator QuestionMarkRoutine()
    {
        quesinMark.SetActive(true);
        yield return new WaitForSeconds(1f);
        quesinMark.SetActive(false);
    }

    private void OnDrawGizmos()
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

    public void VisitLever()
    {
        _animator.SetTrigger("lever");
    }

    public void FinishVisiting()
    {
        humanState = HumanState.Waiting;
    }

    public void VisitElectricPanel()
    {
        _animator.SetTrigger("electricPanel");
    }

    public void ElectricPanelDeath()
    {
        _animator.SetTrigger("electricity");
        Death();
    }

    public virtual void VisitPit()
    {
    }

    public virtual void FinishVisitPit()
    {
    }

    public void VisitTimer(float animationSpeed)
    {
        _animator.SetTrigger("timerOn");
        _animator.SetFloat("timerSpeed", animationSpeed);
    }

    public void FinishVisitTimer()
    {
        _animator.SetTrigger("timerOff");
    }

    public virtual Battery GetBattery()
    {
        return null;
    }

    public virtual void StartTakingBattery(Battery battery)
    {
        _animator.SetTrigger("batteryTake");
    }

    public virtual void PutBattery()
    {
    }

    public virtual bool TryTakeBattery(Battery battery)
    {
        return false;
    }

    public virtual void RemoveBattery()
    {
    }

    public void Teleport(Vector3 position, Vector3 direction)
    {
        WalkTo(position + direction);
        transform.position = position;
    }
}
