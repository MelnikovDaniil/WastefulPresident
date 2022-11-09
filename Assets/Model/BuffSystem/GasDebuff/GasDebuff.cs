using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buff/GasDebff")]
public class GasDebuff : ScriptableBuff
{
    public ParticleSystem poisonParticles;

    public override TimedBuff InitializeBuff(GameObject obj)
    {
        return new TimedGasDebuff(this, obj);
    }
}