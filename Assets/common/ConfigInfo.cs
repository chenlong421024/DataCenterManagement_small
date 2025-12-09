using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigInfo
{

    private string iniPath = Application.streamingAssetsPath + "/config.ini";
    //public string appName = "数字机房可视化管理";//软件名称
    private string appVersion = "1.0";//软件版本
                                      //public string copyright = "";//版权声明
                                      //public string exitPass = "122333";//退出密码
                                      //public bool bootStart = true;//开机自启
                                      //[HideInInspector]
    private string ip = "127.0.0.0";//开机自启
    private string arer;
    private string posMax;
    private string UMax;

    public string IP
    {
        get
        {
            return ip;
        }
    }

    private int port = 12306;//开机自启
    public int Port
    {
        get
        {
            return port;
        }
    }
    public int appWidth;//软件宽度
    public int appHeight;//软件高度
                         // Start is called before the first frame update
                         //void Start()
                         //{
                         //    ReadConfigFromIni();//从配置文件中读取配置
                         //    WriteConfigToIni();//写配置文件
                         //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
    private static ConfigInfo instance;
    public static ConfigInfo Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ConfigInfo();
                instance.ReadConfigFromIni();
            }

            return instance;
        }
    }

    /// <summary>
    /// 从配置文件中读取配置
    /// </summary>
    private void ReadConfigFromIni()
    {
        CheckConfigFile();//检测配置文件是否存在

        INIParser iNIParser = new INIParser();
        iNIParser.Open(iniPath);

        //appName = iNIParser.ReadValue("System", "软件名称", "华远");
        ip = iNIParser.ReadValue("Net", "ip", ip);
        port = iNIParser.ReadValue("Net", "port", port);
        appVersion = iNIParser.ReadValue("System", "version", "0.0");
        arer = iNIParser.ReadValue("Pos", "arer", arer);
        posMax = iNIParser.ReadValue("Pos", "posMax", posMax);
        UMax = iNIParser.ReadValue("Pos", "UMax", UMax);
        iNIParser.Close();
    }

    /// <summary>
    /// 修改配置文件
    /// </summary>
    private void WriteConfigToIni()
    {
        CheckConfigFile();//检测配置文件是否存在

        INIParser iNIParser = new INIParser();

        //增加/修改
        iNIParser.Open(iniPath);
        //iNIParser.WriteValue("[Test]", "说明", "配置文件写入测试");
        
        iNIParser.Close();
    }

    /// <summary>
    /// 从配置文件中删除配置项
    /// 示例:
    /// DelConfigFromIni("Test", "name")
    /// DelConfigFromIni("Test")
    /// </summary>
    private void DelConfigFromIni(String group, string item)
    {
        CheckConfigFile();//检测配置文件是否存在

        INIParser iNIParser = new INIParser();

        //删除
        iNIParser.Open(iniPath);
        iNIParser.KeyDelete(group, item);
        iNIParser.SectionDelete(group);
        iNIParser.Close();
    }

    /// <summary>
    /// 检测配置文件是否存在，不存在则抛出文件不存在的异常
    /// </summary>
    private void CheckConfigFile()
    {
        if (!File.Exists(iniPath))
        {
            throw (new FileNotFoundException(string.Format("配置文件 {0} 不存在", iniPath)));
        }
    }

    public string GetPoseArer()
    {
        return arer;
    }
    public string GetPosMax()
    {
        return posMax;
    }
    public string GetUMax()
    {
        return UMax;
    }
}
