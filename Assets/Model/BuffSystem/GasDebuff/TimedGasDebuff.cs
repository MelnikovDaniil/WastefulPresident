using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedGasDebuff : TimedBuff
{
    private Character character;

    public TimedGasDebuff(ScriptableBuff scriptableBuff, GameObject obj) : base(scriptableBuff, obj)
    {
        character = obj.GetComponent<Character>();
    }

    protected override void ApplyEffect()
    {
        //throw new System.NotImplementedException();
    }

    public override void End()
    {
        character.GetComponent<Animator>().SetTrigger("bomb");
        character.Death();
    }
}
