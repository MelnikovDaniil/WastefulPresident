using System;
using UnityEngine;

public static class SoundtrackMapper
{
    private const string MapperName = "Soundtrack";
    private const int NumberLocationLevels = 32;

    public static void SetSoundtrack(string soundtrackName, string fromLevel)
    {
        PlayerPrefs.SetString($"{MapperName}FromLevel{fromLevel}", soundtrackName);
    }

    public static string GetSoundtrack(string fromLevel)
    {
        if (int.TryParse(fromLevel, out var levelNumber))
        {
            var locationFirstLevel = levelNumber / NumberLocationLevels * NumberLocationLevels + 1;
            return PlayerPrefs.GetString($"{MapperName}FromLevel{locationFirstLevel}");
        }
        else
        {
            return PlayerPrefs.GetString($"{MapperName}FromLevel{fromLevel}");
        }
    }
}
