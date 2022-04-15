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
    private int bitmask;
    // Start is called before the first frame update

    private void Awake()
    {
        characterLayer = LayerMask.NameToLayer("Characters");
        bitmask = LayerMask.GetMask("Characters");
        _animator = GetComponent<Animator>();
    }

    public new void Start()
    {
        base.Start();
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

    public bool InTossPosition(Human human)
    {
        var colliers = Physics2D.OverlapBoxAll(tossPlaceOffset + transform.position, tossPlaceSize, 0, bitmask);
        return colliers.Any(x => x.gameObject == human.gameObject);
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
                if (tossColliders.Contains(humanCollider))
                {
                    human.transform.position = tossPlaceOffset + transform.position + new Vector3(0, Mathf.Abs(human.checkGroundOffsetY));
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
                    var trampoline = colliders.FirstOrDefault(x => x.gameObject.layer == 8)?
                        .GetComponent<Trampoline>();
                    if (!trampoline || !trampoline.InTossPosition(human))
                    {
                        human.HideTarget();
                        human.Disable(disableTime);
                        human.OnLanding = null;
                    }
                    else
                    {
                        human.CheckTrampoline();
                    }
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
