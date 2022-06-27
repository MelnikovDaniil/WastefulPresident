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
    public LayerMask tossingLayers;


    private Animator _animator;

    private BoxCollider2D _collider;
    // Start is called before the first frame update

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = gameObject.AddComponent<BoxCollider2D>();
        _collider.size = tossPlaceSize;
        _collider.offset = tossPlaceOffset;
        _collider.isTrigger = true;
    }

    public new void Start()
    {
        base.Start();
    }

    public override void UpdateState()
    {
        if (isActive)
        {
            TossUp();
            _collider.enabled = true;
            arrow.SetActive(true);
        }
        else
        {
            _collider.enabled = false;
            arrow.SetActive(false);
        }
    }

    public bool InTossPosition(Creature creature)
    {
        var colliers = Physics2D.OverlapBoxAll(tossPlaceOffset + transform.position, tossPlaceSize, 0, tossingLayers);
        return colliers.Any(x => x.gameObject == creature.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive && (tossingLayers & (1 << collision.gameObject.layer)) > 0)
        {
            TossUp();
        }
    }

    private void TossUp()
    {
        var colliers = Physics2D.OverlapBoxAll(tossPlaceOffset + transform.position, discardingSize, 0, tossingLayers);
        var tossColliders = Physics2D.OverlapBoxAll(tossPlaceOffset + transform.position, tossPlaceSize, 0, tossingLayers);

        if (tossColliders.Any())
        {
            _animator.SetTrigger("push");
            foreach (var humanCollider in colliers)
            {
                var creature = humanCollider.GetComponent<Creature>();
                if (creature.characterState == CharacterState.Dead)
                {
                    continue;
                }

                humanCollider.attachedRigidbody.velocity = Vector2.zero;
                if (tossColliders.Contains(humanCollider))
                {
                    creature.transform.position = tossPlaceOffset + transform.position + new Vector3(0, Mathf.Abs(creature.checkGroundOffsetY));
                    humanCollider.attachedRigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
                    creature.GetComponent<Animator>().SetTrigger("trampolineJump");
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
                creature.OnLanding = (IEnumerable<Collider2D> colliders) =>
                {
                    var trampoline = colliders.FirstOrDefault(x => x.gameObject.layer == 8)?
                        .GetComponent<Trampoline>();
                    if (!trampoline || !trampoline.InTossPosition(creature))
                    {
                        creature.HideTarget();
                        if (creature is Zombie)
                        {
                            creature.Disable(disableTime * 2);
                        }
                        else
                        {
                            creature.Disable(disableTime);
                        }
                        creature.OnLanding = null;
                    }
                    else
                    {
                        creature.CheckTrampoline();
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
