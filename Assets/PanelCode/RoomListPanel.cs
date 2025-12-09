using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoomListPanel : PanelBase
{
    private Text[] textPaiMingName;
    private Text[] textPaiMingTime;
    private Transform content;
    private GameObject roomPrefab;
    //private Button closeBtn;
    //private Button newBtn;
    private Button quickJoinBtn;
    private Object[] objRoomPrefabs;
    #region 生命周期
    /// <summary> 初始化 </summary>
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "RoomListPanel";
        layer = PanelLayer.Panel;
        textPaiMingName = new Text[3];
        textPaiMingTime = new Text[3];
        PlayerDataMgr.GetInstance().playerData[0].state = PlayStatueEnum.Login;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        //获取Transform
        Transform skinTrans = skin.transform;
        Transform listTrans = skinTrans.Find("ListImage");
        quickJoinBtn = listTrans.Find("QuickJoinBtn").GetComponent<Button>();
        quickJoinBtn.onClick.AddListener(OnQuicJoinBtnClick);
        Transform winTrans = skinTrans.Find("PaiMingImage");
        //获取列表栏部件
        Transform scroolRect = listTrans.Find("ScrollRect");
        content = scroolRect.Find("Content");


        for (int i = 1; i < 4; i++)
        {
            string panel = "Panel" + i;
            string textN = panel + "/TextName" + i;
            string textT = panel + "/TextTime" + i;
            textPaiMingName[i - 1] = winTrans.Find(textN).GetComponent<Text>();
            textPaiMingTime[i - 1] = winTrans.Find(textT).GetComponent<Text>();
        }

        //按钮事件
        for (int i = 0; i < 50; i++)
        {
            Transform trans = content.Find("RoomPrefab" + i.ToString());
            Button btn = trans.Find("JoinButton").GetComponent<Button>();
            btn.name = i.ToString();   //改变按钮的名字，以便给OnJoinBtnClick传参
            btn.onClick.AddListener(delegate ()
            {
                OnJoinBtnClick(btn.name);
            }
            );
        }
        

        //roomPrefab = content.Find("RoomPrefab").gameObject;
        //roomPrefab.SetActive(false);

        //closeBtn = listTrans.Find("CloseBtn").GetComponent<Button>();
        //newBtn = listTrans.Find("NewBtn").GetComponent<Button>();
        //reflashBtn = listTrans.Find("ReflashBtn").GetComponent<Button>();

        //reflashBtn.onClick.AddListener(OnReflashClick);
        //newBtn.onClick.AddListener(OnNewClick);
        //closeBtn.onClick.AddListener(OnCloseClick);
        //监听
        NetMgr.srvConn.msgDist.AddListener("GetAchieve", RecvGetAchieve);
        NetMgr.srvConn.msgDist.AddListener("GetRoomList", RecvGetRoomList);
        NetMgr.srvConn.msgDist.AddListener("Notice", RecvNotice);

        //发送查询
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetRoomList");
        NetMgr.srvConn.Send(protocol);

        protocol = new ProtocolBytes();
        protocol.AddString("GetAchieve");
        NetMgr.srvConn.Send(protocol);
       
    }

    public override void OnClosing()
    {
        NetMgr.srvConn.msgDist.DelListener("GetAchieve", RecvGetAchieve);
        NetMgr.srvConn.msgDist.DelListener("GetRoomList", RecvGetRoomList);
        NetMgr.srvConn.msgDist.DelListener("Notice", RecvNotice);
    }

    #endregion


    //收到GetAchieve协议
    public void RecvGetAchieve(ProtocolBase protocol)
    {
        //解析协议
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        //处理
        for (int i = 0; i < 3; i++)
        {
            // protocolRet.AddString(RoomMgr.plays[i].name);
            //protocolRet.AddFloat(RoomMgr.plays[i].bestResord);
            textPaiMingName[i].text = proto.GetString(start, ref start);
            textPaiMingTime[i].text = proto.GetFloat(start, ref start).ToString();
        }
    }


    //收到GetRoomList协议
    public void RecvGetRoomList(ProtocolBase protocol)
    {
        //清理
        //ClearRoomUnit();
        //解析协议
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        for (int i = 0; i < 50; i++)
        {
            int num = proto.GetInt(start, ref start);
            int status = proto.GetInt(start, ref start);
            ReflashRoomUnit(i, num, status);
        }
    }
    public void RecvNotice(ProtocolBase protocol)
    {
        //清理
        //ClearRoomUnit();
        //解析协议
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        string noticeStr= proto.GetString(start, ref start);
        PanelMgr.instance.OpenPanel<TipPanel>("", noticeStr);
    }

    public void ClearRoomUnit()
    {
        for (int i = 0; i < content.childCount; i++)
            if (content.GetChild(i).name.Contains("Clone"))
                Destroy(content.GetChild(i).gameObject);
    }


    //创建一个房间单元
    //参数 i，房间序号（从0开始）
    //参数num，房间里的玩家数
    //参数status，房间状态，1-准备中 2-战斗中
    public void GenerateRoomUnit(int i, int num, int status)
    {
        //添加房间
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (i + 1) * 110);
        GameObject o = Instantiate(roomPrefab);
        o.transform.SetParent(content);
        o.SetActive(true);
        //房间信息
        Transform trans = o.transform;
        Text nameText = trans.Find("nameText").GetComponent<Text>();
        Text countText = trans.Find("CountText").GetComponent<Text>();
        Text statusText = trans.Find("StatusText").GetComponent<Text>();
        nameText.text = "序号：" + (i + 1).ToString();
        countText.text = "人数：" + num.ToString();
        if (status == 1)
        {
            statusText.color = Color.black;
            statusText.text = "状态：准备中";
        }
        else
        {
            statusText.color = Color.red;
            statusText.text = "状态：开战中";
        }
    }


    //刷新按钮
    public void OnReflashClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetRoomList");
        NetMgr.srvConn.Send(protocol);
    }

    //加入按钮
    public void OnJoinBtnClick(string name)
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("EnterRoom");
        int roomBtn = int.Parse(name);
        protocol.AddInt(roomBtn);
        NetMgr.srvConn.Send(protocol, OnJoinBtnBack);
        Debug.Log("请求进入房间" + name);
    }

    //加入按钮返回
    public void OnJoinBtnBack(ProtocolBase protocol)
    {
        //解析参数
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        //处理
        if (ret == 0)
        {
            //PanelMgr.instance.OpenPanel<TipPanel>("", "成功进入房间!");
            int rn= proto.GetInt(start, ref start);
            int seat= proto.GetInt(start, ref start);
            PlayerDataMgr.GetInstance().playerData[0].roomNum = rn+1;
            PlayerDataMgr.GetInstance().playerData[0].seat = seat;
            //PlayerDataMgr.GetInstance().playerData[0].seat = seat;
            PanelMgr.instance.OpenPanel<RoomPanel>("",rn,seat);
            Close();
        }
        else
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "进入房间失败");
        }
    }
    public void OnQuicJoinBtnClick()
    {
        OnJoinBtnClick("-1");
    }
    //新建按钮
    public void OnNewClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("CreateRoom");
        NetMgr.srvConn.Send(protocol, OnNewBack);
    }

    //新建按钮返回
    public void OnNewBack(ProtocolBase protocol)
    {
        //解析参数
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        //处理
        if (ret == 0)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "创建成功!");
            PanelMgr.instance.OpenPanel<RoomPanel>("");
            Close();
        }
        else
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "创建房间失败！");
        }
    }

    //登出按钮
    public void OnCloseClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Logout");
        NetMgr.srvConn.Send(protocol, OnCloseBack);
    }

    //登出返回
    public void OnCloseBack(ProtocolBase protocol)
    {
        PanelMgr.instance.OpenPanel<TipPanel>("", "登出成功！");
        //PanelMgr.instance.OpenPanel<LoginPanel>("", "");
        NetMgr.srvConn.Close();
    }
    public void ReflashRoomUnit(int i, int num, int status)
    {
        Transform trans = content.Find("RoomPrefab" + i.ToString());
        Text countText = trans.Find("CountText").GetComponent<Text>();
        Text statusText = trans.Find("StatusText").GetComponent<Text>();
        Button btn = trans.Find(i.ToString()).GetComponent<Button>();
        countText.text = "人数:"+num.ToString();
        if (status == 1)
        {
            statusText.color = Color.blue;
            statusText.text = "状态：准备中";
            if (num >= 4)
            {
                btn.enabled = false;
                btn.image.color = Color.gray;

            }
            else
            {
                btn.enabled = true;
                btn.image.color = Color.white;
            }
        }
        else
        {
            statusText.color = Color.red;
            statusText.text = "状态：开战中";
            btn.enabled = false;
            btn.image.color = Color.gray;
        }
        //按钮事件
        //btn.name = i.ToString();   //改变按钮的名字，以便给OnJoinBtnClick传参
        //btn.onClick.AddListener(delegate ()
        //{
        //    OnJoinBtnClick(btn.name);
        //}
        //);
    }
    public override void OnEsc()
    {
        OnCloseClick();
        PanelMgr.instance.OpenPanel<StartPanel>("");
        Close();
    }
}