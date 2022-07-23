using UnityEngine;

public class Door : PowerConsumer
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public override void TurnEnergy()
    {
        base.TurnEnergy();
        if (isActive)
        {
            SoundManager.PlaySound("DoorOpen");
        }
        else
        {
            SoundManager.PlaySound("DoorClose");
        }
    }

    public override void UpdateState()
    {
        _animator.SetBool("open", isActive);
    }
}
