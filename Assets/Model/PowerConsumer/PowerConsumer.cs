using UnityEngine;

public abstract class PowerConsumer : MonoBehaviour
{
    public bool isActive;

    public void Start()
    {
        UpdateState();
    }

    public virtual void TurnEnergy()
    {
        isActive = !isActive;
        UpdateState();
    }

    public abstract void UpdateState();
}
