using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreatureSoundManagerComponent : SoundManagerComponent
{
    public List<SoundGroup> soundGroups;

    public void PlayCreatureSound(string groupName)
    {
        var group = soundGroups.FirstOrDefault(x => x.groupName == groupName);
        if (group == null || !group.audioClips.Any())
        {
            Debug.LogError($"{gameObject.name} does not contains sounds for \"{groupName}\" Group");
        }
        else
        {
            PlayRandomClip(group.audioClips);
        }
    }
}
