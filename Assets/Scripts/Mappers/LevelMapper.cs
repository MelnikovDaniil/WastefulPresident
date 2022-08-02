using UnityEngine;

public enum LevelStatus
{
    Locked,
    Avaliable,
    Complete,
}

public static class LevelMapper
{
    private const string MapperName = "Level";

    public static LevelStatus GetStatus(string levelName)
    {
        return (LevelStatus)PlayerPrefs.GetInt(MapperName + "Status" + levelName, 0);
    }

    public static void Open(string levelName)
    {
        PlayerPrefs.SetInt(MapperName + "Status" + levelName, (int)LevelStatus.Avaliable);
    }
    public static void Complete(string levelName)
    {
        PlayerPrefs.SetInt(MapperName + "Status" + levelName, (int)LevelStatus.Complete);
    }

    public static int GetAttempts(string levelName)
    {
        return PlayerPrefs.GetInt(MapperName + "Attempts" + levelName, 0);
    }

    public static void AddAttempt(string levelName)
    {
        var attempts = PlayerPrefs.GetInt(MapperName + "Attempts" + levelName, 0);
        PlayerPrefs.SetInt(MapperName + "Attempts" + levelName, attempts);
    }

    public static void ResetAttempt(string levelName)
    {
        PlayerPrefs.SetInt(MapperName + "Attempts" + levelName, 0);
    }

    public static void SetComicsBeforeLevel(string comicsName, string levelName)
    {
        PlayerPrefs.SetString(MapperName + "ComicsNameBeforeLevel" + levelName, comicsName);
    }

    public static string GetComicsBeforeLevel(string levelName)
    {
        return PlayerPrefs.GetString(MapperName + "ComicsNameBeforeLevel" + levelName, null);
    }
}
