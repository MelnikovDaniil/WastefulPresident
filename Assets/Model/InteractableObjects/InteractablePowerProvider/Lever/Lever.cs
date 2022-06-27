using UnityEngine;

public class Lever : InteractrablePowerProvider
{
    private Animator _animator;

    public void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public override void StartInteraction(ICharacterVisitor visitor)
    {
        base.StartInteraction(visitor);
        visitor.VisitLever();
    }

    public override void UpdateState()
    {
        _animator.SetBool("enable", isActive);
    }
}
