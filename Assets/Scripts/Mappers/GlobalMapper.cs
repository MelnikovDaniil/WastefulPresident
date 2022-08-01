using UnityEngine;

public static class GlobalMapper
{
    private const string MapperName = "Global";

    public static bool IsFirstPlay()
    {
        var isFirstPlay = PlayerPrefs.GetInt(MapperName + "FirstPlay", 0) == 0;
        PlayerPrefs.SetInt(MapperName + "FirstPlay", 1);
        return isFirstPlay;
    }
}
