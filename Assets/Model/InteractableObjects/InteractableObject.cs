using System.Collections;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public float interactionTime = 0;

    private IVisitor visitor;

    public virtual void StartInteraction(IVisitor visitor)
    {
        this.visitor = visitor;
        StartCoroutine(InteractionRoutine());
    }

    public abstract void SuccessInteraction(IVisitor visitor);

    private IEnumerator InteractionRoutine()
    {
        yield return new WaitForSeconds(interactionTime);
        visitor.FinishVisiting();
        SuccessInteraction(visitor);
    }
}
