using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

//网络链接
public class Connection
{
    //常量
    const int BUFFER_SIZE = 1024;
    //Socket
    private Socket socket;
    //Buff
    private byte[] readBuff = new byte[BUFFER_SIZE];
    private int buffCount = 0;
    //沾包分包
    private Int32 msgLength = 0;
    private byte[] lenBytes = new byte[sizeof(Int32)];
    //协议
    public ProtocolBase proto;
    //心跳时间
    public float lastTickTime = 0;
    public float heartBeatTime = 30;

    public long lastTickTimeLong = long.MinValue;

    //消息分发
    public MsgDistribution msgDist = new MsgDistribution();
    ///状态
    public enum Status
    {
        None,
        Connected,
    };
    public Status status = Status.None;

    //连接服务端
    public bool Connect(string host, int port)
    {
        try
        {
            //socket
            socket = new Socket(AddressFamily.InterNetwork,
                      SocketType.Stream, ProtocolType.Tcp);
            //Connect
            socket.Connect(host, port);
            //BeginReceive
            socket.BeginReceive(readBuff, buffCount,
                      BUFFER_SIZE - buffCount, SocketFlags.None,
                      ReceiveCb, readBuff);
            Debug.Log("连接成功");
            //状态
            status = Status.Connected;
            lastTickTimeLong = Sys.GetTimeStamp();
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("连接失败:" + e.Message);
            //msgDist.DelAllOnceListener();
            return false;
        }
    }

    //关闭连接
    public bool Close()
    {
        try
        {
            socket.Close();
            NetMgr.srvConn.status = Connection.Status.None;
            return true;
        }
        catch (Exception e)
        {
            //msgDist.DelAllOnceListener();
            Debug.Log("关闭失败:" + e.Message);
            return false;
        }
    }

    //接收回调
    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            int count = socket.EndReceive(ar);
            buffCount = buffCount + count;
            ProcessData();
            socket.BeginReceive(readBuff, buffCount,
                     BUFFER_SIZE - buffCount, SocketFlags.None,
                     ReceiveCb, readBuff);
        }
        catch (Exception e)
        {
            Debug.Log("ReceiveCb失败:" + e.Message);
            //PanelMgr.instance.CloseAllPanel();
            Close();
        }
    }

    //消息处理
    private void ProcessData()
    {
        try
        {
            //沾包分包处理
            if (buffCount < sizeof(Int32))
                return;
            //包体长度
            Array.Copy(readBuff, lenBytes, sizeof(Int32));
            msgLength = BitConverter.ToInt32(lenBytes, 0);
            if (buffCount < msgLength + sizeof(Int32))
                return;
            //协议解码
            ProtocolBase protocol = proto.Decode(readBuff, sizeof(Int32), msgLength);
            Debug.Log("ProcessData 收到消息 " + protocol.GetDesc());
            lock (msgDist.msgList)
            {
                msgDist.msgList.Add(protocol);
            }
            //清除已处理的消息
            int count = buffCount - msgLength - sizeof(Int32);
            Array.Copy(readBuff, sizeof(Int32) + msgLength, readBuff, 0, count);
            buffCount = count;
            if (buffCount > 0)
            {
                ProcessData();
            }
        }
        catch (Exception e)
        {
            Debug.Log("ProcessData:" + e.Message);
            //PanelMgr.instance.CloseAllPanel();
            Close();
        }
    }


    public bool Send(ProtocolBase protocol)
    {
        try
        {
            if (status != Status.Connected)
            {
                //Debug.Log("[Connection]还没链接就发送数据是不好的");
                //PlayerDataMgr.GetInstance().playerData[0].state = PlayStatueEnum.None;
                //PanelMgr.instance.CloseAllPanel();
                //PanelMgr.instance.OpenPanel<StartPanel>("StartPanel");
                //PanelMgr.instance.OpenPanel<TipPanel>("", "服务器断开！");
                ////msgDist.DelAllOnceListener();
                Close();
                return true;
            }
            byte[] b = protocol.Encode();
            byte[] length = BitConverter.GetBytes(b.Length);
            byte[] sendbuff = length.Concat(b).ToArray();
            socket.Send(sendbuff);
        }
        catch (Exception ex)
        {
            //PlayerDataMgr.GetInstance().playerData[0].state = PlayStatueEnum.None;
            //PanelMgr.instance.CloseAllPanelOne();
            //PanelMgr.instance.OpenPanel<StartPanel>("StartPanel");
            //PanelMgr.instance.OpenPanel<TipPanel>("", "服务器断开！");
            //msgDist.DelAllOnceListener();
            Close();
            Debug.Log(ex);
        }
        Debug.Log("发送消息 " + protocol.GetName());
        return true;
    }

    public bool Send(ProtocolBase protocol, string cbName, MsgDistribution.Delegate cb)
    {
        if (status != Status.Connected)
            return false;
        msgDist.AddOnceListener(cbName, cb);
        return Send(protocol);
    }

    public bool Send(ProtocolBase protocol, MsgDistribution.Delegate cb)
    {
        string cbName = protocol.GetName();
        return Send(protocol, cbName, cb);
    }

    public void Update()
    {
        //消息
        msgDist.Update();
        //心跳 每30秒发送一次
        if (status == Status.Connected)
        {
            long timeNow = Sys.GetTimeStamp();
            float lastBeatTime = Time.time - lastTickTime;
            if (timeNow - lastTickTimeLong > 165+(long)lastBeatTime)//5 秒的延时
            {
                //PanelMgr.instance.CloseAllPanelOne();
                //PanelMgr.instance.OpenPanel<StartPanel>("StartPanel");
                //PanelMgr.instance.OpenPanel<TipPanel>("", "超时断开！");
                Close();
                return;
            }
            if (Time.time - lastTickTime > heartBeatTime)
            {
                ProtocolBase protocol = NetMgr.GetHeatBeatProtocol();
                Send(protocol);
                lastTickTime = Time.time;
                lastTickTimeLong= Sys.GetTimeStamp();
                Debug.Log("发送BeatTime消息 " + protocol.GetDesc());
            }
        }
    }
}