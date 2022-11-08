using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedGasDebuff : TimedBuff
{
    private GasDebuff gasDebuff;
    private Character character;
    private ParticleSystem particles;

    public TimedGasDebuff(ScriptableBuff scriptableBuff, GameObject obj) : base(scriptableBuff, obj)
    {
        gasDebuff = scriptableBuff as GasDebuff;
        character = obj.GetComponent<Character>();
    }

    protected override void ApplyEffect()
    {
        particles = GameObject.Instantiate(gasDebuff.poisonParticles, character.transform);
        //throw new System.NotImplementedException();
    }

    protected override void DispelEffect()
    {
        GameObject.Destroy(particles.gameObject);
    }

    public override void End()
    {
        GameObject.Destroy(particles.gameObject);
        character.GetComponent<Animator>().SetTrigger("bomb");
        character.Death();
    }
}
