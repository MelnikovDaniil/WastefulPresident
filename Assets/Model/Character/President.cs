using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class President : Character, IPresidentVisitor
{
    [NonSerialized]
    public bool isLocked;
    [Space]
    [Range(0f, 1f)]
    public float sendOrderChanse = 1;

    [Space]
    public Material colorMaterial;

    [Space]
    public float watchingClockFromTime = 15f;

    private float currentIdleTime = 0;

    public void Update()
    {
        if (!isLocked && !DialogueManager.isWorking && characterState != CharacterState.Dead)
        {
            if (characterState == CharacterState.Waiting)
            {
                currentIdleTime += Time.deltaTime;
                if (currentIdleTime >= watchingClockFromTime)
                {
                    WatchClock();
                }
            }
            else
            {
                currentIdleTime = 0;
            }
        }
    }

    public override void SetColor(Color color)
    {
        colorMaterial.SetColor("_Color", color);
        colorMaterial.SetFloat("_Thickness", 0f);
    }

    public override void ShowColor()
    {
        colorMaterial.SetFloat("_Thickness", 1.5f);
    }

    public override void HideColor()
    {
        colorMaterial.SetFloat("_Thickness", 0f);
    }

    public void SendOrder()
    {
        if (sendOrderChanse > Random.value)
        {
            _animator.SetTrigger("order");
        }
    }

    public void PlayAnimation(string animName)
    {
        _animator.Play(animName);
    }

    public new void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.TryGetComponent<ColliderDialogueTrigger>(out var dialogueTrigger))
        {
            dialogueTrigger.TriggerDialogue();
        }
    }

    private void WatchClock()
    {
        _animator.SetTrigger("clock");
        currentIdleTime = 0;
    }
}
