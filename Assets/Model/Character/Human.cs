using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Human : MonoBehaviour, IVisitor
{
    public event Action OnDeath;
    public float speed;
    public float jumpForce;

    [Space]
    public float targetStopDistanceX = 0.1f;
    public float targetStopDistanceY = 1f;
    public float checkGroundOffsetY = -1.8f;
    public float checkFroundRadius = 0.3f;
    public float interactRadius = 0.5f;
    public Vector2 checkWallOffset = new Vector2(1.2f, 0.5f);

    [Space]
    public float samePositionTime = 0.1f;
    public float samePositionDistance = 0.05f;

    [Space]
    public SpriteRenderer characterColor;

    [NonSerialized]
    public HumanState humanState;

    protected Rigidbody2D _rigidbody;
    protected Animator _animator;

    protected Vector2? target;

    protected float currentPositionTime;
    protected float previosPositionX;

    protected bool isGrounded;

    protected bool inFrontOfWall;
    protected bool jumpDelay;

    private void Awake()
    {
        characterColor.gameObject.SetActive(false);
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }


    private void Update()
    {
        if (humanState != HumanState.Dead && !jumpDelay && inFrontOfWall && isGrounded)
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

    public virtual void TryInteract()
    {
        var collider = Physics2D.OverlapCircleAll(transform.position, interactRadius)
            .FirstOrDefault(x => x.GetComponent<InteractableObject>());
        if (collider != null)
        {
            var interactableObject = collider.GetComponent<InteractableObject>();
            humanState = HumanState.Acivating;
            interactableObject.StartInteraction(this);


            //else if (collider.TryGetComponent<PitInteractableObject>(out var pit))
            //{
            //    _animator.SetTrigger("pit");
            //    transform.localScale = new Vector3(
            //        Mathf.Abs(transform.localScale.x) * Mathf.Sign(pit.transform.localScale.x),
            //        transform.localScale.y,
            //        transform.localScale.z);
            //    GetComponent<SpriteRenderer>().sortingOrder += 10;
            //    _rigidbody.velocity = Vector2.zero;
            //    _rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
            //    Death();
            //    _rigidbody.freezeRotation = true;
            //}
        }
    }

    public virtual void Death()
    {
        humanState = HumanState.Dead;
        OnDeath?.Invoke();
        _rigidbody.sharedMaterial = null;
        _rigidbody.freezeRotation = false;
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

    public virtual void SetTarget(Vector2 target)
    {
        if (humanState != HumanState.Dead)
        {
            humanState = HumanState.MovingToInteract;
            this.target = target;
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
                    humanState = HumanState.Waiting;
                    target = null;
                    _rigidbody.velocity = Vector2.zero;
                    _animator.SetBool("run", false);
                    _animator.SetBool("walk", false);
                    currentPositionTime = 0;
                }
            }
        }
    }

    protected void CheckGround()
    {
        var position = new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY);
        var colliders = Physics2D.OverlapCircleAll(position, checkFroundRadius)
            .Where(x => x.gameObject.layer == 6 || x.gameObject.layer == 7);

        isGrounded = colliders.Any();
        _animator.SetBool("grounded", isGrounded);
    }

    protected void CheckWall()
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

    public void VisitPit()
    {
        _animator.SetTrigger("pit");
        GetComponent<SpriteRenderer>().sortingOrder += 10;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void FinishVisitPit()
    {
        _animator.SetBool("fall", true);
        GetComponent<SpriteRenderer>().sortingOrder -= 10;
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
