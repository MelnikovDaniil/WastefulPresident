using System.Collections;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public float interactionTime = 0;

    public bool forCharacter = true;
    public bool forAgent = true;

    private ICharacterVisitor visitor;

    public virtual void StartInteraction(ICharacterVisitor visitor)
    {
        this.visitor = visitor;
        StartCoroutine(InteractionRoutine());
    }

    public abstract void SuccessInteraction(ICharacterVisitor visitor);

    private IEnumerator InteractionRoutine()
    {
        yield return new WaitForSeconds(interactionTime);
        visitor.FinishVisiting();
        SuccessInteraction(visitor);
    }
}
