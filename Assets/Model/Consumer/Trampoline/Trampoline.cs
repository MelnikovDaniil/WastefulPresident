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
    public float discardingForce = 5f;

    private int characterLayer;
    // Start is called before the first frame update
    public new void Start()
    {
        base.Start();
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
            //sprteRenderer.sprite = pressedSprite;
        }
        else
        {
            //sprteRenderer.sprite = releasedSprite;
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
                    humanCollider.attachedRigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
                }
                else
                {
                    var discardingVector = humanCollider.transform.position - transform.position;
                    humanCollider.attachedRigidbody.AddForce(
                        new Vector3(5, 1).normalized * discardingForce,
                        ForceMode2D.Impulse);
                }
                human.OnLanding = (IEnumerable<Collider2D> colliders) =>
                {
                    if (!colliders.Any(x => x.gameObject.layer == 8))
                    {
                        human.HideTarget();
                        human.OnLanding = null;
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
