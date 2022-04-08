using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Trampoline : PowerConsumer
{
    public Vector3 tossPlaceOffset;
    public Vector2 tossPlaceSize;
    public Vector2 discardingSize;
    public float force = 5f;
    public float disableTime = 1.5f;
    public Vector2 minMaxDiscardingForce = new Vector2(2f, 7f);

    [Space]
    public GameObject arrow;

    private Animator _animator;
    private int characterLayer;
    // Start is called before the first frame update
    public new void Awake()
    {
        //base.Start();
        _animator = GetComponent<Animator>();
        characterLayer = LayerMask.NameToLayer("Characters");
        var boxCollider = gameObject.AddComponent<BoxCollider2D>();
        boxCollider.size = tossPlaceSize;
        boxCollider.offset = tossPlaceOffset;
        boxCollider.isTrigger = true;
    }

    public override void UpdateState()
    {
        if (isActive)
        {
            TossUp();
            arrow.SetActive(true);
        }
        else
        {
            arrow.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive && collision.gameObject.layer == characterLayer)
        {
            TossUp();
        }
    }

    private void TossUp()
    {
        var bitmask = 1 << characterLayer;
        var colliers = Physics2D.OverlapBoxAll(tossPlaceOffset + transform.position, discardingSize, 0, bitmask);
        var tossColliders = Physics2D.OverlapBoxAll(tossPlaceOffset + transform.position, tossPlaceSize, 0, bitmask);

        if (tossColliders.Any())
        {
            _animator.SetTrigger("push");
            foreach (var humanCollider in colliers)
            {
                var human = humanCollider.GetComponent<Human>();
                if (human.humanState == HumanState.Dead)
                {
                    continue;
                }

                humanCollider.attachedRigidbody.velocity = Vector2.zero;
                human.CheckTrampilineSide();
                if (tossColliders.Contains(humanCollider))
                {
                    humanCollider.attachedRigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
                    human.GetComponent<Animator>().SetTrigger("trampolineJump");
                }
                else
                {
                    var maxHumanDistance = discardingSize.x / 2;
                    var forceCoof = 1.0f - Mathf.Clamp01(
                        Mathf.Abs(humanCollider.transform.position.x - transform.position.x) / maxHumanDistance);
                    var discardingVector = humanCollider.transform.position - transform.position;
                    var calculatedForce = minMaxDiscardingForce.x
                        + (minMaxDiscardingForce.y - minMaxDiscardingForce.x) * forceCoof;


                    humanCollider.attachedRigidbody.AddForce(
                        discardingVector.normalized * calculatedForce,
                        ForceMode2D.Impulse);
                }
                human.OnLanding = (IEnumerable<Collider2D> colliders) =>
                {
                    human.HideTarget();
                    human.Disable(disableTime);
                    human.OnLanding = null;
                };
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(tossPlaceOffset + transform.position, tossPlaceSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(tossPlaceOffset + transform.position, discardingSize);
    }
}
