using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PanelMgr : MonoBehaviour
{
     
    //单例
    public static PanelMgr instance;
    //画板
    private GameObject canvas;
    //面板
    public Dictionary<string, PanelBase> dict;
    //层级
    private Dictionary<PanelLayer, Transform> layerDict;
    //当前面板
    public string strCurrentPanel;
    //面板栈
    public List<string> panelList;  
    //开始
    public void Awake()
    {
        instance = this;
        InitLayer();
        dict = new Dictionary<string, PanelBase>();
        panelList = new List<string>();
        //Messenger.AddListener(GameEvent.Break, CloseAllPanel);
    }

    //初始化层
    private void InitLayer()
    {
        //画布
        canvas = GameObject.Find("Canvas");
        if (canvas == null)
            Debug.LogError("panelMgr.InitLayer fail, canvas is null");
        //各个层级
        layerDict = new Dictionary<PanelLayer, Transform>();
        foreach (PanelLayer pl in Enum.GetValues(typeof(PanelLayer)))
        {
            string name = pl.ToString();
            Transform transform = canvas.transform.Find(name);  
            layerDict.Add(pl, transform);
        }
    }
    public Transform GetPanelTra( PanelLayer pl)
    {
        return layerDict[pl];
    }
    //打开面板
    public void OpenPanel<T>(string skinPath, params object[] args) where T : PanelBase
    {
        //已经打开
        string name = typeof(T).ToString(); 
        if(dict.ContainsKey(name))
            return;
        //面板脚本
        PanelBase panel = canvas.AddComponent<T>();
        panel.Init(args);
        dict.Add(name, panel);
        //Debug.Log(name);
       //加载皮肤
       skinPath = (skinPath != "" ? skinPath : panel.skinPath);
        GameObject skin = Resources.Load<GameObject>(skinPath);
        if (skin == null)
            Debug.LogError("panelMgr.OpenPanel fail, skin is null,skinPath = " + skinPath);
        panel.skin = (GameObject)Instantiate(skin);
        //坐标
        Transform skinTrans = panel.skin.transform;
        PanelLayer layer = panel.layer;
        Transform parent = layerDict[layer];
        skinTrans.SetParent(parent, false);
        //panel的生命周期
        panel.OnShowing();
        //anm
        panel.OnShowed();
    }

    //关闭面板
    public void ClosePanel(string name)
    {
        if (!dict.ContainsKey(name))
            return;
        PanelBase panel = (PanelBase)dict[name];
        if (panel == null)
            return;

        panel.OnClosing();
        dict.Remove(name);
        panel.OnClosed();
        //for (int i = 0; i < stackPanel.Count; i++)
        //{
        //    Debug.Log("pop  " + stackPanel.ToArray()[i]);
        //}
        GameObject.Destroy(panel.skin);
        Component.Destroy(panel);
    }
    public void CloseAllPanel()
    {
        for (int i = 0; i < PanelMgr.instance.panelList.Count; i++)
        {
            string str = panelList[i];
            PanelBase panel = dict[str];
            panel.OnEsc();
        }
    }
    public void CloseAllPanelOne()
    {
        for (int i = 0; i < PanelMgr.instance.panelList.Count; i++)
        {
            string str = panelList[i];
            PanelBase panel = dict[str];
            panel.Close();
        }
    }
    public void OnEsc()
    {
        if (PlayerDataMgr.GetInstance().playerData[0].state==PlayStatueEnum.Fight)
        {
            LeaveRoom();
        }
        else
        {
            string str = panelList[panelList.Count - 1];
            PanelBase panel = dict[str];
            panel.OnEsc();
            if (str == "GameOverTipPanel")
            {
                LeaveRoom();
            }
        }
    }
    public void OnLogoutBack(ProtocolBase protocol)
    {

    }
    public void LeaveRoom()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("LeaveRoom");
        NetMgr.srvConn.Send(protocol, OnGameCloseBack);
    }


    public void OnGameCloseBack(ProtocolBase protocol)
    {
        //获取数值
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        //处理
        if (ret == 0)
        {
            //PanelMgr.instance.OpenPanel<TipPanel>("", "退出成功!");
            PlayerDataMgr.GetInstance().playerData[0].state = PlayStatueEnum.Login;
            PanelMgr.instance.OpenPanel<RoomListPanel>("");
            //Close();
        }
        else
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "退出失败！");
        }
    }
}


///分层类型
public enum PanelLayer
{
    //面板
    Panel,
    //提示
    Tips,
}
