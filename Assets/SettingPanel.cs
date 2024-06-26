using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    public Toggle volumeToggle;
    public Toggle vibrationToggle;

    public void Start()
    {
        volumeToggle.isOn = SettingsMapper.GetVolume();
        vibrationToggle.isOn = SettingsMapper.GetVibration();

        volumeToggle.onValueChanged.AddListener(ChangeVolume);
        vibrationToggle.onValueChanged.AddListener(ChangeVibration);
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void ChangeVolume(bool isOn)
    {
        SettingsMapper.SetVolume(isOn);
        SoundManager.SetSoundMuted(!isOn);
        SoundManager.SetMusicMuted(!isOn);

        SoundManager.PlaySound("checkBox");
    }

    private void ChangeVibration(bool isOn)
    {
        SettingsMapper.SetVibration(isOn);
        Vibration.VibrateIfOn();
        SoundManager.PlaySound("checkBox");
    }
}
