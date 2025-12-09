  using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoginPanel : PanelBase
{
    private InputField idInput;
    private InputField pwInput;
    private Button loginBtn;
    private Button regBtn;
    private Button findPWbtn;
    private Button backBtn;

    #region 生命周期
    //初始化
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "LoginPanel";
        layer = PanelLayer.Panel;
        PlayerDataMgr.GetInstance().Clear();
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        idInput = skinTrans.Find("IDInput").GetComponent<InputField>();
        pwInput = skinTrans.Find("PWInput").GetComponent<InputField>();
        loginBtn = skinTrans.Find("LoginBtn").GetComponent<Button>();
        //regBtn = skinTrans.Find("RegBtn").GetComponent<Button>();
        findPWbtn= skinTrans.Find("FindPWBtn").GetComponent<Button>();
        backBtn = skinTrans.Find("BtnBack").GetComponent<Button>();
        loginBtn.onClick.AddListener(OnLoginClick);
        //regBtn.onClick.AddListener(OnRegClick);
        findPWbtn.onClick.AddListener(OnFindPWClick);
        backBtn.onClick.AddListener(OnEsc);
    }
    #endregion

    public void OnRegClick()
    {
        PanelMgr.instance.OpenPanel<RegPanel>("");
        //Close();
    }
    public void OnFindPWClick()
    {
        PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "请联系管理员!");
        //Close();
    }

    public void OnLoginClick()
    {
        if (!InputValidCheck())
            return;
        //读取数据库

        /*/连接服务器
        //if (NetMgr.srvConn.status != Connection.Status.Connected)
        //{
        //    //string host = "127.0.0.1";
        //    //int port = 1234;
        //    //string host = Root.IP;
        //    string host = ConfigInfo.Instance.IP;
        //    if (host == string.Empty)
        //        return;
        //    int port = ConfigInfo.Instance.Port;
        //    NetMgr.srvConn.proto = new ProtocolBytes();
        //    if (!NetMgr.srvConn.Connect(host, port))
        //        PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "连接服务器失败!");
        //}
        ////发送
        //ProtocolBytes protocol = new ProtocolBytes();
        //protocol.AddString("Login");
        //protocol.AddString(idInput.text);
        //protocol.AddString(pwInput.text);
        //Debug.Log("发送 OnLoginClick" + protocol.GetDesc());
        //NetMgr.srvConn.Send(protocol, OnLoginBack);*/
    }
    public bool InputValidCheck()
    {
        if (pwInput.text == "" || idInput.text == "")
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "账号或密码为空!");
            return false;
        }
        if (Check.CheckGonghaoNum(idInput.text)==false )
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "账号输入格式有误!");
            return false;
        }
        if (!Check.CheckPassWold(pwInput.text))
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "密码输入错误\n1、字母开头\n2、只能有字母或数字\n3、6 - 18位长度");
            return false;
        }
        return true;
    }
    public void OnLoginBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        //Debug.Log("发送 OnLoginClick" + protocol.GetDesc());
        if (ret == 0)
        {
            DoCSVSQL.Instance.ISOnline = true;
            //Messenger.Broadcast(GameEvent.LoadData);
            DoCSVSQL.Instance.ReadCSV();
            Messenger.Broadcast(GameEvent.LoadFuWuQi);
            string name = proto.GetString(start, ref start);
            //float bestRecord = proto.GetFloat(start, ref start);
            //int ranking = proto.GetInt(start, ref start);
            PlayerDataMgr.GetInstance().playerData[0].Init(name);
            //开始游戏
            //PanelMgr.instance.OpenPanel<RoomListPanel>("");
            PanelMgr.instance.OpenPanel<TipPanel>("",skinPath, "登录成功!");
 
            //GameMgr.instance.id = idInput.text;
            Close();
        }
        else
        {
            NetMgr.srvConn.Close();
            PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "登录失败，请检查用户名密码!");
        }
    }
    public override void OnEsc()
    {
        PanelMgr.instance.OpenPanel<StartPanel>("");
        Close();
    }
    public override void OnClosing()
    {
        if (loginBtn.onClick != null)
        {
            loginBtn.onClick.RemoveAllListeners();
        }
        if (backBtn.onClick != null)
        {
            backBtn.onClick.RemoveAllListeners();
        }
        if (findPWbtn.onClick != null)
        {
            findPWbtn.onClick.RemoveAllListeners();
        }
    }
}