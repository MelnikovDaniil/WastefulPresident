using System;
using System.Linq;
using UnityEngine;

public class Human : MonoBehaviour
{
    public event Action OnDeath;
    public float speed;
    public float jumpForce;

    [Space]
    public float checkGroundOffsetY = -1.8f;
    public float checkFroundRadius = 0.3f;
    public float interactRadius = 0.5f;

    [NonSerialized]
    public bool isDead;

    protected Rigidbody2D _rigidbody;
    protected Animator _animator;

    protected bool isGrounded;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }


    public void TryInteract()
    {
        var collider = Physics2D.OverlapCircleAll(transform.position, interactRadius)
            .FirstOrDefault(x => x.GetComponent<InteractableObject>());
        if (collider != null)
        {
            collider.GetComponent<InteractableObject>().Interect();
        }
    }

    public virtual void Death()
    {
        OnDeath?.Invoke();
        isDead = true;
        _rigidbody.freezeRotation = false;
        GetComponent<SpriteRenderer>().color += Color.black;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDead && collision.tag == "DeathCollider")
        {
            Death();
        }
    }

    protected void CheckGround()
    {
        var position = new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY);
        var colliders = Physics2D.OverlapCircleAll(position, checkFroundRadius).Where(x => x.gameObject.layer == 6);

        isGrounded = colliders.Any();
        _animator.SetBool("grounded", isGrounded);
    }

    private void OnDrawGizmos()
    {
        var position = new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position, checkFroundRadius);
    }
}
