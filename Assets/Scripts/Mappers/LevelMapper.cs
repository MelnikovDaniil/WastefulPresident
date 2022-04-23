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
}
