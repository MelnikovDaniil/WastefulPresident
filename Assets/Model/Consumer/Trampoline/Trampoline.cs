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

    private BoxCollider2D _collider;
    // Start is called before the first frame update

    private void Awake()
    {
        characterLayer = LayerMask.NameToLayer("Characters");
        bitmask = LayerMask.GetMask("Characters");
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

    public bool InTossPosition(Character character)
    {
        var colliers = Physics2D.OverlapBoxAll(tossPlaceOffset + transform.position, tossPlaceSize, 0, bitmask);
        return colliers.Any(x => x.gameObject == character.gameObject);
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
                var character = humanCollider.GetComponent<Character>();
                if (character.characterState == CharacterState.Dead)
                {
                    continue;
                }

                humanCollider.attachedRigidbody.velocity = Vector2.zero;
                if (tossColliders.Contains(humanCollider))
                {
                    character.transform.position = tossPlaceOffset + transform.position + new Vector3(0, Mathf.Abs(character.checkGroundOffsetY));
                    humanCollider.attachedRigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
                    character.GetComponent<Animator>().SetTrigger("trampolineJump");
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
                character.OnLanding = (IEnumerable<Collider2D> colliders) =>
                {
                    var trampoline = colliders.FirstOrDefault(x => x.gameObject.layer == 8)?
                        .GetComponent<Trampoline>();
                    if (!trampoline || !trampoline.InTossPosition(character))
                    {
                        character.HideTarget();
                        character.Disable(disableTime);
                        character.OnLanding = null;
                    }
                    else
                    {
                        character.CheckTrampoline();
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
