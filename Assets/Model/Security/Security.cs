using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Security : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float targetStopDistance = 0.1f;
    public float presidentStopDistance = 0.1f;

    [Space]
    public float checkGroundOffsetY = -0.5f;
    public float checkGroundOffsetX = 0.5f;
    public float checkFroundRadius = 0.3f;

    [NonSerialized]
    public Vector2? target;

    private Rigidbody2D _rigidbody;
    private bool isGrounded;
    private bool inFrontOfWall;
    private bool jumpDelay;
    private bool isFollowPresedent;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!jumpDelay && inFrontOfWall && isGrounded)
        {
            isGrounded = false;
            jumpDelay = true;
            _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            StartCoroutine(JumpDelayRoutine());
        }
    }

    private IEnumerator JumpDelayRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        jumpDelay = false;
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            var side = Mathf.Sign(target.Value.x - transform.position.x);
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * side, transform.localScale.y, 0);
            _rigidbody.velocity = new Vector2(side * speed, _rigidbody.velocity.y);
            var targetDistance = Mathf.Abs(transform.position.x - target.Value.x);
            if (isFollowPresedent && targetDistance < presidentStopDistance)
            {
                isFollowPresedent = false;
                target = null;
                _rigidbody.velocity = Vector2.zero;
            }
            if (targetDistance < targetStopDistance)
            {
                target = null;
                _rigidbody.velocity = Vector2.zero;
            }

            CheckGround();
            CheckWall();
        }
    }


    public void FollowPresedent(Vector2 target)
    {
        isFollowPresedent = true;
        this.target = target;
    }

    private void CheckGround()
    {
        var position = new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY);
        var colliders = Physics2D.OverlapCircleAll(position, checkFroundRadius).Where(x => x.gameObject.layer == 6);

        isGrounded = colliders.Any();
    }

    private void CheckWall()
    {
        var position = new Vector2(transform.position.x + checkGroundOffsetX * Mathf.Sign(transform.localScale.x), transform.position.y);
        var colliders = Physics2D.OverlapCircleAll(position, checkFroundRadius).Where(x => x.gameObject.layer == 6);

        inFrontOfWall = colliders.Any();
    }

    private void OnDrawGizmos()
    {
        var groundPosition = new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundPosition, checkFroundRadius);

        var wallPosition = new Vector2(transform.position.x + checkGroundOffsetX * Mathf.Sign(transform.localScale.x), transform.position.y);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(wallPosition, checkFroundRadius);
    }
}
