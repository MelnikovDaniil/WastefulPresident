using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Trampoline : PowerConsumer
{
    public Vector3 pressurePlacePosition;
    public Vector2 pressureSize;

    public float pressureCheckPeriod = 0.5f;
    public float force = 5f;
    // Start is called before the first frame update
    public new void Start()
    {
        base.Start();
        var boxCollider = gameObject.AddComponent<BoxCollider2D>();
        boxCollider.size = pressureSize;
        boxCollider.offset = pressurePlacePosition;
        boxCollider.isTrigger = true;
    }

    public override void UpdateState()
    {
        if (isActive)
        {
            //sprteRenderer.sprite = pressedSprite;
        }
        else
        {
            //sprteRenderer.sprite = releasedSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive && collision.gameObject.layer == LayerMask.NameToLayer("Characters"))
        {
            var colliers = Physics2D.OverlapBoxAll(pressurePlacePosition + transform.position, pressureSize, 0);
            var people = colliers.Where(x => x.GetComponent<Human>() && x.GetComponent<Human>().humanState != HumanState.Dead);
            if (people.Any())
            {
                foreach (var humanCollider in people)
                {
                    var human = humanCollider.GetComponent<Human>();
                    humanCollider.attachedRigidbody.velocity = Vector2.zero;
                    humanCollider.attachedRigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
                    human.OnLanding += human.HideTarget;
                    human.OnLanding += () => { human.OnLanding = null; };
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(pressurePlacePosition + transform.position, pressureSize);
    }
}
