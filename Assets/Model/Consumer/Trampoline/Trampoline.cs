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
        StartCoroutine(CheckHumanEnterRoutine());
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

    private IEnumerator CheckHumanEnterRoutine()
    {
        while (isActive)
        {
            yield return new WaitForSeconds(pressureCheckPeriod);
            var colliers = Physics2D.OverlapBoxAll(pressurePlacePosition + transform.position, pressureSize, 0);
            var people = colliers.Where(x => x.GetComponent<Human>() && x.GetComponent<Human>().humanState != HumanState.Dead);
            if (people.Any())
            {
                foreach (var human in people)
                {
                    human.attachedRigidbody.velocity = Vector2.zero;
                    human.attachedRigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
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
