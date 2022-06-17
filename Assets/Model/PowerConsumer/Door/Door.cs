using UnityEngine;

public class Door : PowerConsumer
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public override void UpdateState()
    {
        _animator.SetBool("open", isActive);
    }
}
