using UnityEngine;

public static class SettingsMapper
{
    private const string MapperName = "Settings";

    public static bool GetVolume()
    {
        return PlayerPrefs.GetInt(MapperName + "Volume", 1) == 1;
    }

    public static void SetVolume(bool value)
    {
        PlayerPrefs.SetInt(MapperName + "Volume", value ? 1 : 0);
    }

    public static bool GetVibration()
    {
        return PlayerPrefs.GetInt(MapperName + "Vibration", 1) == 1;
    }

    public static void SetVibration(bool value)
    {
        PlayerPrefs.SetInt(MapperName + "Vibration", value ? 1 : 0);
    }

    public static void SetPostProcess(bool value)
    {
        PlayerPrefs.SetInt(MapperName + "PostProcess", value ? 1 : 0);
    }
}
