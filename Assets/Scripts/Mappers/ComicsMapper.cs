using UnityEngine;

public static class ComicsMapper
{
    private const string MapperName = "Comics";

    public static void SetComicsToShow(string comicsName)
    {
        PlayerPrefs.SetString(MapperName + "Current", comicsName);
    }

    public static string GetComicsToShow()
    {
        return PlayerPrefs.GetString(MapperName + "Current");
    }

    public static void SetAfterShow(string comicsName)
    {
        PlayerPrefs.SetString(MapperName + "AfterShow", comicsName);
    }

    public static string GetAfterShow()
    {
        return PlayerPrefs.GetString(MapperName + "AfterShow", "LevelMenu");
    }

    public static void Open(string comicsName)
    {
        PlayerPrefs.SetInt(MapperName + comicsName, 1);
    }

    public static bool IsAvailable(string comicsName)
    {
        return PlayerPrefs.GetInt(MapperName + comicsName, 0) == 1;
    }
}
