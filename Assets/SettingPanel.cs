using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    public Slider volumeSlider;
    public Toggle vibrationToggle;
    public Toggle postProcessToggle;

    public void Start()
    {
        volumeSlider.value = SettingsMapper.GetVolume();
        vibrationToggle.isOn = SettingsMapper.GetVibration();
        postProcessToggle.isOn = SettingsMapper.GetPostProcess();

        volumeSlider.onValueChanged.AddListener(ChangeVolume);
        vibrationToggle.onValueChanged.AddListener(ChangeVibration);
        postProcessToggle.onValueChanged.AddListener(ChangePostProcess);
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void ChangeVolume(float value)
    {
        SettingsMapper.SetVolume(value);
        SoundManager.SetSoundVolume(value);
        SoundManager.SetMusicVolume(value);

        SoundManager.PlaySound("volumeSlider2");
    }

    private void ChangeVibration(bool isOn)
    {
        SettingsMapper.SetVibration(isOn);
        Vibration.VibrateIfOn();
        SoundManager.PlaySound("checkBox");
    }

    private void ChangePostProcess(bool isOn)
    {
        SettingsMapper.SetPostProcess(isOn);
        SoundManager.PlaySound("checkBox");
    }
}
