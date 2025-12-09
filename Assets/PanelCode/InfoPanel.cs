using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoPanel : PanelBase
{

    private Button updateBtn;
    private Button closeBtn;
    private Text textVio;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "InfoPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        updateBtn = skinTrans.Find("ButtonUpdate").GetComponent<Button>();
        closeBtn = skinTrans.Find("ButtonClose").GetComponent<Button>();
        textVio = skinTrans.Find("TextVio").GetComponent<Text>();
        updateBtn.onClick.AddListener(OnUpdateClick);
        closeBtn.onClick.AddListener(OnEsc);
    }
    #endregion

    public void OnUpdateClick()
    {
        /* if (NetMgr.srvConn.status != Connection.Status.Connected)
         {
             //string host = "127.0.0.1";
             //int port = 1234;
             string host = Root.IP;
             if (host == string.Empty)
                 return;
             int port = Root.Port;
             NetMgr.srvConn.proto = new ProtocolBytes();
             if (!NetMgr.srvConn.Connect(host, port))
                 PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "连接服务器失败!");
         }
         ProtocolBytes protocol = new ProtocolBytes();
         protocol.AddString("Update");
         int v =int.Parse(textVio.text);
         protocol.AddInt(v);

         NetMgr.srvConn.Send(protocol, RecvUpdate); */
        PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "当前版本是最新版本!");
        //Close();
    }
    public void RecvUpdate(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int index = proto.GetInt(start, ref start);
        if (index == -1)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "StartPanel", "当前版本已是最新版本！");
            NetMgr.srvConn.Close();
        }
        else if(index == 0)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "StartPanel", "版本有跟新\n当前版本不支持实时更新\n请到 www.cl1986.cn:81\n下载更新，谢谢！");
            NetMgr.srvConn.Close();
        }
    }
    public override void OnEsc()
    {
        Close();
    }
    public override void OnClosing()
    {
        if (updateBtn.onClick != null)
        {
            updateBtn.onClick.RemoveAllListeners();
        }
        //NetMgr.srvConn.d(protocol, RecvUpdate);
    }
}