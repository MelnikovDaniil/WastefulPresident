using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class LevelMapper
{
    private const string MapperName = "Level";

    public static string GetCurrentLevel()
    {
        return PlayerPrefs.GetString(MapperName + "Current", null);
    }

    public static void SetCurrentLevel(string levelName)
    {
        PlayerPrefs.SetString(MapperName + "Current", levelName);
        Open(levelName);
    }

    public static bool IsOpen(string levelName)
    {
        return PlayerPrefs.GetInt(MapperName + "IsOpen" + levelName, 0) == 1;
    }

    public static void Open(string levelName)
    {
        PlayerPrefs.SetInt(MapperName + "IsOpen" + levelName, 1);
    }
}
