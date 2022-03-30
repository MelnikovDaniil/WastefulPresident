using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pit : InteractableObject, IComplexPositioning
{
    public Vector2 centerInteractionPosition;
    public Vector2 interactionSize;

    public Collider2D bridgeCollider;

    public Animator timerAnimator;

    private bool isBusy;

    private void Start()
    {
        timerAnimator.SetFloat("speed", 1.0f / interactionTime);
    }

    public Vector2 GetPositionForInteraction(Human human)
    {
        var firstPos = (Vector2)transform.position + centerInteractionPosition;
        var secondPos = (Vector2)transform.position + centerInteractionPosition * new Vector2(-1, 1);

        if (Vector2.Distance(human.transform.position, firstPos) < Vector2.Distance(human.transform.position, secondPos))
        {
            return firstPos;
        }

        return secondPos;
    }

    public override void StartInteraction(IVisitor visitor)
    {
        if (!isBusy)
        {
            isBusy = true;
            base.StartInteraction(visitor);
            visitor.VisitPit();
            bridgeCollider.enabled = true;
            GetComponent<BoxCollider2D>().enabled = false;
            timerAnimator.SetTrigger("show");
        }
    }

    public override void SuccessInteraction(IVisitor visitor)
    {
        bridgeCollider.enabled = false;
        isBusy = false;
        GetComponent<BoxCollider2D>().enabled = true;
        visitor.FinishVisitPit();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + centerInteractionPosition, interactionSize);
        Gizmos.DrawWireCube((Vector2)transform.position + centerInteractionPosition * new Vector2(-1, 1), interactionSize);
    }
}
