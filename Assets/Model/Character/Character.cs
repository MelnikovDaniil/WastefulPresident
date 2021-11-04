using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float speed;
    public float jumpForce;

    [Space]
    public float checkGroundOffsetY = -1.8f;
    public float checkFroundRadius = 0.3f;

    private Rigidbody2D _rigidbody;
    private float horizontalMove = 0;

    private bool isGrounded;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.W))
        {
            isGrounded = false;
            _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        horizontalMove = Input.GetAxisRaw("Horizontal") * speed;
    }

    public void FixedUpdate()
    {
        _rigidbody.velocity = new Vector2(horizontalMove, _rigidbody.velocity.y);
        CheckGround();
    }

    private void CheckGround()
    {
        var position = new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY);
        var colliders = Physics2D.OverlapCircleAll(position, checkFroundRadius).Where(x => x.gameObject.layer == 6);

        isGrounded = colliders.Any();
    }

    private void OnDrawGizmos()
    {
        var position = new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position, checkFroundRadius);
    }
}
