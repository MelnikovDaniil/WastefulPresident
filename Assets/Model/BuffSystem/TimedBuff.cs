using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimedBuff
{
    protected float Duration;
    protected int EffectStackes;

    public ScriptableBuff Buff { get; }

    protected readonly GameObject Obj;

    public bool IsFinished;

    public TimedBuff(ScriptableBuff buff, GameObject obj)
    {
        Buff = buff;
        Obj = obj;
    }

    public void Tick(float delta)
    {
        if (!IsFinished)
        {
            Duration -= delta;
            if (Duration <= 0)
            {
                End();
                IsFinished = true;
            }
        }
    }

    public void Activate()
    {
        if (Buff.IsEffecteStacked || Duration <= 0)
        {
            ApplyEffect();
            EffectStackes++;
        }

        if (Buff.IsDurationStacked || Duration <= 0)
        {
            Duration += Buff.Duration;
        }
    }

    public void Dispel()
    {
        Duration = 0;
        IsFinished = true;
        DispelEffect();
    }

    protected abstract void ApplyEffect();
    protected abstract void DispelEffect();
    public abstract void End();

}
