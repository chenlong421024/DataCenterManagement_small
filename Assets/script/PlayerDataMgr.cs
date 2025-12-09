using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家数据
/// </summary>
public class PlayerDataMgr
{
    public PlayerData[] playerData;
    private static PlayerDataMgr instance=new PlayerDataMgr();
    public PlayerDataMgr()
    {
        playerData = new PlayerData[4];
        for (int i = 0; i < 4; i++)
        {
            playerData[i] = new PlayerData();
        }
    }
    public static PlayerDataMgr GetInstance()
    {
        return instance;
    }
    public void Clear()
    {
        for (int i = 0; i < 4; i++)
        {
            playerData[i].Clear();
        }
    }
}
