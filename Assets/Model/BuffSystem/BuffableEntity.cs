using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffableEntity : MonoBehaviour
{
    public GasDebuff gasDebuff;

    private Character character;
    private readonly Dictionary<ScriptableBuff,  TimedBuff> _buffs = new Dictionary<ScriptableBuff, TimedBuff>();

    private void Start()
    {
        character = GetComponent<Character>();
    }

    private void Update()
    {
        foreach (var buff in _buffs.Values)
        {
            buff.Tick(Time.deltaTime);
            if (buff.IsFinished)
            {
                _buffs.Remove(buff.Buff);
            }
        }
    }

    public void AddBuff(TimedBuff buff)
    {
        if (_buffs.TryGetValue(buff.Buff, out var existingBuff))
        {
            existingBuff.Activate();
        }
        else
        {
            _buffs.Add(buff.Buff, buff);
            buff.Activate();
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (character.characterState != CharacterState.Dead)
        {
            switch (collision.tag)
            {
                case "Gas":
                    AddBuff(gasDebuff.InitializeBuff(gameObject));
                    break;
            }
        }

    }
}
