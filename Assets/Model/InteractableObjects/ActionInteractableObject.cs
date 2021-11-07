using UnityEngine.Events;

public class ActionInteractableObject : InteractableObject
{
    public UnityEvent action;
    public bool singleRun;

    public override void Interect()
    {
        action?.Invoke();
        if (singleRun)
        {
            action.RemoveAllListeners();
        }
    }
}
