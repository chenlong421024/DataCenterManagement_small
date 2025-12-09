using UnityEngine;
using System.Collections;
using System.Net;
using System;

public class Root : MonoBehaviour 
{

    // Use this for initialization
    //private  static string ip = string.Empty;
    //public static string IP
    //{
    //    set
    //    {
    //        ip = value;
    //    }
    //    get
    //    {
    //        if (ip == string.Empty)
    //        {
    //            PanelMgr.instance.OpenPanel<TipPanel>("",  "正在链接服务器....!");
    //            //StartCoroutine(coroutine);
    //        }
    //        return ip;
    //    }
    //}
    //public static int Port = 1234;
    //IEnumerator coroutine;
    void Start () 
    {
        Application.runInBackground = true;
        PanelMgr.instance.OpenPanel<StartPanel>("");
    }

    void Update()
    {
        //直接写数据库，不通过服务端加工
        //NetMgr.Update();
    }
    //IEnumerator GetIp()
    //{
    //    try
    //    {

    //        IPHostEntry host = Dns.GetHostEntry("www.cl1986.cn");
    //        //foreach (var va in host.AddressList)
    //        //{
    //        //    Debug.Log(va.ToString()+ "www.cl1986.cn");
    //        //}
    //        IP = host.AddressList[0].ToString();
    //    }
    //    catch (Exception ex)
    //    {
    //        PanelMgr.instance.OpenPanel<TipPanel>("", "请检查您的网络是否良好！");
    //        Debug.Log(ex);
    //    }
    //    yield break;
    //} 
}
