using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : InteractrablePowerProvider
{
    public float workingTime = 5;
    private Animator _animator;

    public bool isBusy;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public new void Start()
    {
        _animator.SetFloat("chargingTime", 1 / interactionTime);
        _animator.SetFloat("workingTime", 1 / workingTime);
        base.Start();
    }

    public override void StartInteraction(IVisitor visitor)
    {
        if (!isBusy)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            base.StartInteraction(visitor);
            _animator.SetBool("charging", true);
            isBusy = true;
        }
    }

    public override void SuccessInteraction(IVisitor visitor)
    {
        base.SuccessInteraction(visitor);
        StartCoroutine(ResetRoutine());
    }

    public override void UpdateState()
    {
        _animator.SetBool("charging", false);
    }

    private IEnumerator ResetRoutine()
    {
        yield return new WaitForSeconds(workingTime);
        GetComponent<BoxCollider2D>().enabled = true;
        TurnEnergy();
        isActive = false;
        isBusy = false;
    }
}
