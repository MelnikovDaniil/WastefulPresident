using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class MoneyMapper
{
    private const string MapperName = "Money";

    public static int Get()
    {
        return PlayerPrefs.GetInt(MapperName, 0);
    }

    public static void Add(int money)
    {
        var currentMoney = Get();
        PlayerPrefs.SetInt(MapperName, currentMoney + money);
    }
}
