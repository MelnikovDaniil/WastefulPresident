using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pit : InteractableObject, IComplexPositioning, IDoubleVisiting
{
    public Vector2 centerInteractionPosition;
    public Vector2 interactionSize;

    public Collider2D bridgeCollider;

    public Animator timerAnimator;

    private ICharacterVisitor pitVisitor;
    private Collider2D triggerCollider;

    private bool isBusy;

    private void Awake()
    {
        triggerCollider = GetComponent<BoxCollider2D>();
    }
    private void Start()
    {
        timerAnimator.SetFloat("speed", 1.0f / interactionTime);
    }

    public Vector2 GetPositionForInteraction(Character character)
    {
        var firstPos = (Vector2)transform.position + centerInteractionPosition;
        var secondPos = (Vector2)transform.position + centerInteractionPosition * new Vector2(-1, 1);

        if (Vector2.Distance(character.transform.position, firstPos) < Vector2.Distance(character.transform.position, secondPos))
        {
            return firstPos;
        }

        return secondPos;
    }

    public override void StartInteraction(ICharacterVisitor visitor)
    {
        if (!isBusy)
        {
            isBusy = true;
            pitVisitor = visitor;
            base.StartInteraction(visitor);
            visitor.VisitPit();
            bridgeCollider.enabled = true;
            triggerCollider.enabled = false;
            timerAnimator.SetBool("show", true);
        }
    }

    public override void SuccessInteraction(ICharacterVisitor visitor)
    {
        if (isBusy)
        {
            bridgeCollider.enabled = false;
            isBusy = false;
            pitVisitor = null;
            triggerCollider.enabled = true;
            visitor.FinishVisitPit();
            timerAnimator.SetBool("show", false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + centerInteractionPosition, interactionSize);
        Gizmos.DrawWireCube((Vector2)transform.position + centerInteractionPosition * new Vector2(-1, 1), interactionSize);
    }

    public bool IsDoubleVisiting(ICharacterVisitor visitor)
    {
        return visitor == pitVisitor;
    }

    public void DoubleVisit(ICharacterVisitor visitor)
    {
        StopAllCoroutines();
        visitor.FinishVisiting();
        SuccessInteraction(pitVisitor);
    }
}
