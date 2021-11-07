using UnityEngine;

public class SoundManagerComponent : MonoBehaviour
{
    public void PlaySound(string name)
    {
        SoundManager.PlaySound(name);
    }

    public void PlayClip(AudioClip clip)
    {
        SoundManager.PlaySound(clip);
    }

    public void PlaySoundNotPausable(string name)
    {
        SoundManager.PlaySoundUI(name);
    }

    public void PlayClipNotPausable(AudioClip clip)
    {
        SoundManager.PlaySoundUI(clip);
    }

    public void ChangeSoundVolume(float volume)
    {
        SoundManager.SetSoundVolume(volume);
    }

    public void ChangeMusicVolume(float volume)
    {
        SoundManager.SetMusicVolume(volume);
    }

    public void ToggleMusicMuted()
    {
        SoundManager.SetMusicMuted(!SoundManager.GetMusicMuted());
    }

    public void ToggleSoundMuted()
    {
        SoundManager.SetSoundMuted(!SoundManager.GetSoundMuted());
    }
}
