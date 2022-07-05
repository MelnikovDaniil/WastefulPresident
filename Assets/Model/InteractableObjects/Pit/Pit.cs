using UnityEngine;

public class Pit : InteractableObject, IComplexPositioning
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
            visitor.VisitPit(() => PitFalling(visitor));
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
            visitor.FinishVisitPit(() => PitFalling(visitor));
            timerAnimator.SetBool("show", false);
        }
    }
    private void PitFalling(ICharacterVisitor visitor)
    {
        this.StopAllCoroutines();
        visitor.FinishVisiting();
        SuccessInteraction(pitVisitor);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + centerInteractionPosition, interactionSize);
        Gizmos.DrawWireCube((Vector2)transform.position + centerInteractionPosition * new Vector2(-1, 1), interactionSize);
    }
}
