using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class SettingsMapper
{
    private const string MapperName = "Settings";

    public static float GetVolume()
    {
        return PlayerPrefs.GetFloat(MapperName + "Volume", 1);
    }

    public static void SetVolume(float value)
    {
        PlayerPrefs.SetFloat(MapperName + "Volume", value);
    }

    public static bool GetVibration()
    {
        return PlayerPrefs.GetFloat(MapperName + "Vibration", 1) == 1;
    }

    public static void SetVibration(bool value)
    {
        PlayerPrefs.SetFloat(MapperName + "Vibration", value ? 1 : 0);
    }

    public static bool GetPostProcess()
    {
        return PlayerPrefs.GetFloat(MapperName + "PostProcess", 1) == 1;
    }

    public static void SetPostProcess(bool value)
    {
        PlayerPrefs.SetFloat(MapperName + "PostProcess", value ? 1 : 0);
    }
}
