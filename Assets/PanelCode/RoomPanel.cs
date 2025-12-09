using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class RoomPanel : PanelBase
{
    private List<Transform> prefabs = new List<Transform>();
    private Button startBtn;
    private Text textStartBtn;
    private Button sendButton;
    Transform[] prefab = new Transform[4];
    Text[] textName = new Text[4];
    Text[] textBestRecord = new Text[4];
    Text[] textRanking = new Text[4];
    Text[] textIsHost = new Text[4];
    //string strRoomNum = "";
    Text textRoomNum;
    Dropdown dropDownItem;
    //int mine=-1;

    //音源
    public AudioSource gameAudioSource;
    //音效
    public AudioClip[] gameClips;
    #region 生命周期
    /// <summary> 初始化 </summary>
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "RoomPanel";
        layer = PanelLayer.Panel;
        gameClips = new AudioClip[4];
        PlayerDataMgr.GetInstance().playerData[0].state = PlayStatueEnum.InRoom;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        textRoomNum = skinTrans.Find("TextRoomNum").GetComponent<Text>();
        PlayerData p = PlayerDataMgr.GetInstance().playerData[0];
        textRoomNum.text = "房间:" + p.roomNum + "   玩家:" + p.name;

        //textRoomNum.text = strRoomNum;
        //组件
        for (int i = 0; i < 4; i++)
        {
            string name = "PlayerPrefab" + i.ToString();
            prefab[i] = skinTrans.Find(name);
            textName[i] = prefab[i].Find("TextName").GetComponent<Text>();
            textBestRecord[i] = prefab[i].Find("TextBestRecord").GetComponent<Text>();
            textRanking[i] = prefab[i].Find("TextRanking").GetComponent<Text>();
            textIsHost[i] = prefab[i].Find("TextIsHost").GetComponent<Text>();
            //prefabs.Add(prefab[i]);
        }
        dropDownItem = skinTrans.Find("Dropdown").GetComponent<Dropdown>();
        Transform t = skinTrans.Find("StartBtn");
        startBtn = t.GetComponent<Button>();
        textStartBtn = t.Find("Text").GetComponent<Text>();
        sendButton = skinTrans.Find("SendButton").GetComponent<Button>();
        //按钮事件
        startBtn.onClick.AddListener(OnStartClick);
        sendButton.onClick.AddListener(OnSendButtonClick);

        gameAudioSource = gameObject.AddComponent<AudioSource>();
        gameAudioSource.spatialBlend = 0;


        gameClips[0] = (AudioClip)Resources.Load("music/zhunbei", typeof(AudioClip));
        gameClips[1] = (AudioClip)Resources.Load("music/hurryup", typeof(AudioClip));
        gameClips[2] = (AudioClip)Resources.Load("music/goodgame", typeof(AudioClip));
        gameClips[3] = (AudioClip)Resources.Load("music/mmgg", typeof(AudioClip));
        //监听
        NetMgr.srvConn.msgDist.AddListener("GetRoomInfo", RecvGetRoomInfo);
        NetMgr.srvConn.msgDist.AddListener("Chat", RecvChat);
        NetMgr.srvConn.msgDist.AddListener("Fight", RecvFight);
        //发送查询
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetRoomInfo");
        NetMgr.srvConn.Send(protocol);
    }

    public override void OnClosing()
    {
        NetMgr.srvConn.msgDist.DelListener("GetRoomInfo", RecvGetRoomInfo);
        NetMgr.srvConn.msgDist.DelListener("Fight", RecvFight);
        NetMgr.srvConn.msgDist.DelListener("Chat", RecvChat);
    }


    public void RecvChat(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int index = proto.GetInt(start, ref start);
        if (index < 0 || index > 3)
        {
            return;
        }
        gameAudioSource.PlayOneShot(gameClips[index]);
    }
    public void RecvGetRoomInfo(ProtocolBase protocol)
    {
        //获取总数
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int count = proto.GetInt(start, ref start);
        //每个处理
        int i = 0;
        if (count > 4)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "网络错误！");
            return;
        }
        for (i = 0; i < 4; i++)
        {
            prefab[i].GetComponent<Image>().color = Color.green;
            foreach (Text t in prefab[i].GetComponentsInChildren<Text>())
            {
                t.transform.localScale = Vector3.zero;
            }
        }
        for (i = 1; i < 4; i++)
        {
            PlayerDataMgr.GetInstance().playerData[i].Clear();
        }
        int index = 1;//从第一个开始
        for (i = 0; i < count; i++)
        {
            int seat = proto.GetInt(start, ref start);
            string id = proto.GetString(start, ref start);
            textName[seat].text = proto.GetString(start, ref start);
            textBestRecord[seat].text = proto.GetFloat(start, ref start).ToString();
            textRanking[seat].text = proto.GetInt(start, ref start).ToString();
            int isOwner = proto.GetInt(start, ref start);
            int state= proto.GetInt(start, ref start);

            foreach (Text t in prefab[seat].GetComponentsInChildren<Text>())
            {
                t.transform.localScale = Vector3.one;
            }
            if (id == PlayerDataMgr.GetInstance().playerData[0].phone)
            {
                PlayerDataMgr.GetInstance().playerData[0].isOwner = (isOwner==1)?true:false;
                PlayerDataMgr.GetInstance().playerData[0].state = (PlayStatueEnum)state;
                if (isOwner == 1)
                {
                    textIsHost[seat].text = "房主";
                    textStartBtn.text = "开始";
                }
                else if (state == (int)PlayStatueEnum.InRoom)
                {
                    textIsHost[seat].text = "等待";
                    textStartBtn.text = "准备";
                }
                else if (state == (int)PlayStatueEnum.Ready)
                {
                    textIsHost[seat].text = "准备";
                    textStartBtn.text = "取消准备";
                }
            }
            else
            {
                if(PlayerDataMgr.GetInstance().playerData[index] ==null)
                    PlayerDataMgr.GetInstance().playerData[index] = new PlayerData();
                PlayerDataMgr.GetInstance().playerData[index].name = textName[seat].text;
                PlayerDataMgr.GetInstance().playerData[index].phone = id;
                PlayerDataMgr.GetInstance().playerData[index].isOwner = (isOwner == 1) ? true : false;
                PlayerDataMgr.GetInstance().playerData[index].state = (PlayStatueEnum)state;
                //MsgName mn = new MsgName(index, PlayerDataMgr.GetInstance().playerData[i + 1].phone, PlayerDataMgr.GetInstance().playerData[i + 1].name);
                if (isOwner == 1)
                {
                    textIsHost[seat].text = "房主";
                }
                else if (state == (int)PlayStatueEnum.InRoom)
                {
                    textIsHost[seat].text = "等待";
                }
                else if (state == (int)PlayStatueEnum.Ready)
                {
                    textIsHost[seat].text = "准备";
                }
                index++;
                //Player.playerOther[i].ranking = textRanking[seat].text;
                //Player.playerOther[i].phone = id;
            }
            prefab[seat].GetComponent<Image>().color=Color.red;
        }
        //清楚多余信息
        for (i=index;i<=3;i++)
        {
            PlayerDataMgr.GetInstance().playerData[index].Clear();
        }
       // Messenger.Broadcast(GameEvent.InitPlayer);
    }

    public void OnCloseClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("LeaveRoom");
        NetMgr.srvConn.Send(protocol, OnCloseBack);
    }


    public void OnCloseBack(ProtocolBase protocol)
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
            PanelMgr.instance.OpenPanel<RoomListPanel>("");
            Close();
        }
        else
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "退出失败！");
        }
    }


    public void OnStartClick()
    {
        int index = PlayerDataMgr.GetInstance().playerData[0].isOwner==true?
            1: (PlayerDataMgr.GetInstance().playerData[0].state== PlayStatueEnum.Ready?3:2);
        switch (index)
        {
            case 1:
                {
                    ProtocolBytes protocol = new ProtocolBytes();
                    protocol.AddString("StartFight");
                    NetMgr.srvConn.Send(protocol, OnStartBack);
                }
                break;
            case 2:
                {
                    ProtocolBytes protocol = new ProtocolBytes();
                    protocol.AddString("Ready");
                    NetMgr.srvConn.Send(protocol);
                    textStartBtn.text = "取消准备";
                }
                break;
            case 3:
                {
                    ProtocolBytes protocol = new ProtocolBytes();
                    protocol.AddString("UnReady");
                    NetMgr.srvConn.Send(protocol);
                    textStartBtn.text = "准备";
                }
                break;
            default:
                break;
        }
        
    }
    public void OnSendButtonClick()
    {
        Debug.Log(dropDownItem.value);
        int i = dropDownItem.value;
        gameAudioSource.PlayOneShot(gameClips[i]);
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Chat");
        //protocol.AddString(Player.phone);
        protocol.AddInt(i);
        NetMgr.srvConn.Send(protocol);
    }
    public void OnStartBack(ProtocolBase protocol)
    {
        //获取数值
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        //处理
        if (ret != 0)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "开始游戏失败，是否都已经准备！");
        }
    }

    public void RecvFight(ProtocolBase protocol)
    {
        Close();
    }

    #endregion

    public override void OnEsc()
    {
        if (NetMgr.srvConn.status == Connection.Status.None)
            Close();
        else
            OnCloseClick();
    }

}