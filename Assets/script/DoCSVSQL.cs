using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using UnityEngine;

public class DoCSVSQL
{
    private static DataTable dt= null;//保存本地数据
    
    private static bool isOnline ;//保存本地数据
    private static DoCSVSQL instance;
    public  static DoCSVSQL Instance
    {
        get
        {
            if (instance == null)
                instance = new DoCSVSQL();
            return instance;
        }
    }
     DoCSVSQL()
    {
        //Messenger.AddListener(GameEvent.LoadData, ReadCSV);
        dt = new DataTable("Server");
        isOnline = false;
    }
    
    public bool ISOnline
    {
        get
        {
            return isOnline;
        }
        set
        {
            isOnline = value;
        }
    }
    public DataTable GetDt()
    {
        if (dt == null)
        {
            dt= new DataTable("Server");
        }
        return dt;
    }
    public void WriteCSV()
    {
        //创建表 设置表名
        //DataTable dt = new DataTable("Sheet1");
        ////创建列 有三列
        //dt.Columns.Add("名字");
        //dt.Columns.Add("年龄");
        //dt.Columns.Add("性别");
        ////创建行 每一行有三列数据
        //DataRow dr = dt.NewRow();
        //dr["名字"] = "张三";
        //dr["年龄"] = "18";
        //dr["性别"] = "nv";
        //dt.Rows.Add(dr);
        //string filePath = Application.streamingAssetsPath + "\\data.csv";//serverinfo.csv
        string filePath = Application.streamingAssetsPath + "\\serverinfo.csv";//serverinfo.csv
        SaveCSV(filePath, dt);
    }
    public void SaveCSV(string CSVPath, DataTable mSheet)
    {
        //判断数据表内是否存在数据
        if (mSheet.Rows.Count < 1)
            return;

        //读取数据表行数和列数
        int rowCount = mSheet.Rows.Count;
        int colCount = mSheet.Columns.Count;

        //创建一个StringBuilder存储数据
        StringBuilder stringBuilder = new StringBuilder();

        //读取数据
        for (int i = 0; i < mSheet.Columns.Count; i++)
        {
            stringBuilder.Append(mSheet.Columns[i].ColumnName + ",");
        }
        stringBuilder.Remove(stringBuilder.Length-1,1);
        stringBuilder.Append("\r\n");
        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < colCount; j++)
            {
                //使用","分割每一个数值
                if (j == 0)
                    mSheet.Rows[i][j] = i+1;
                stringBuilder.Append(mSheet.Rows[i][j] + ",");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);//去掉最后一个逗号
            //使用换行符分割每一行
            if (i != rowCount - 1)
                stringBuilder.Append("\r\n");
        }
        //写入文件
        using (FileStream fileStream = new FileStream(CSVPath, FileMode.Create, FileAccess.Write))
        {
            using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
            {
                textWriter.Write(stringBuilder.ToString());
            }
        }
    }
    public void ReadCSV()
    {
        Debug.Log("ReadCSV  on");
        Debug.Log("ReadCSV");
        if (!isOnline)
        {
            string filePath = Application.streamingAssetsPath + "\\serverinfo.csv";
            dt = OpenCSV(filePath);
        }
        else
        {
            //GetFwqInfo();
            //读数据库
            //DataMgr dataMgr = new DataMgr();
            //if (DataMgr.isCON==true)
            //{
            //    dt = DataMgr.instance.GetInfo();
            //}
            dt = DataMgr.Instance.GetInfo();
            WriteCSV();
        }
        //Debug.Log(dt.Rows[0][0]);
        Debug.Log("ReadCSV  off");
    }

    public DataTable OpenCSV(string filePath)//从csv读取数据返回table
    {
        //DataTable dt = new DataTable();
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                //记录每次读取的一行记录
                string strLine = "";
                //记录每行记录中的各字段内容
                string[] aryLine = null;
                string[] tableHead = null;
                //标示列数
                int columnCount = 0;
                //标示是否是读取的第一行
                bool IsFirst = true;
                //逐行读取CSV中的数据
                while ((strLine = sr.ReadLine()) != null)
                {
                    if (IsFirst == true)
                    {
                        tableHead = strLine.Split(',');
                        IsFirst = false;
                        columnCount = tableHead.Length;
                        //创建列
                        for (int i = 0; i < columnCount; i++)
                        {
                            DataColumn dc = new DataColumn(tableHead[i]);
                            dt.Columns.Add(dc);
                        }
                    }
                    else
                    {
                        aryLine = strLine.Split(',');
                        DataRow dr = dt.NewRow();
                        for (int j = 0; j < columnCount; j++)
                        {
                            dr[j] = aryLine[j];
                        }
                        dt.Rows.Add(dr);
                    }
                }
                if (aryLine != null && aryLine.Length > 0)
                {
                    dt.DefaultView.Sort = tableHead[0] + " " + "asc";
                }
                sr.Close();
                fs.Close();
                return dt;
            }
        }
    }
    public int GetRow(string jigui, string uHigh)
    {
        if (dt != null)
        {
            //int col1=
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][1].ToString() == jigui && dt.Rows[i][2].ToString() == uHigh)
                    return int.Parse( dt.Rows[i][0].ToString()) ;
            }
        }
        return -1;
    }
    //如果登入联网获取服务器上面的数据
    public void  GetFwqInfo()
    {
        //组建消息
        //发送获取数据的消息
        //接收
        //解析
        //保存、
        if (NetMgr.srvConn.status != Connection.Status.Connected)
        {
            //string host = "127.0.0.1";
            //int port = 1234;
            string host = ConfigInfo.Instance.IP;
            if (host == string.Empty)
                return ;
            int port = ConfigInfo.Instance.Port;
            NetMgr.srvConn.proto = new ProtocolBytes();
            if (!NetMgr.srvConn.Connect(host, port))
                PanelMgr.instance.OpenPanel<TipPanel>("",  "连接服务器失败!");
        }
        //发送
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetInfo");
        Debug.Log("发送 +GetInfo" + protocol.GetDesc());
        NetMgr.srvConn.Send(protocol, GetInfo);
        return ;
    }
    public void GetInfo(ProtocolBase protocol)
    {
        Debug.Log("GetInfo  len:"+protocol.GetDesc().Length);
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int count= proto.GetInt(start);
        Debug.Log("收到 + GetInfo" + count);
        for (int i=0;i<3;i++)
        {
            string str = proto.GetString(start, ref start);
            Debug.Log("收到 + GetInfo" + protoName + str);
            //dt = InfoToDt(str);
        }
        //Debug.Log("收到 + GetInfo" + protoName+ str);
        //dt = InfoToDt(str);
    }
    private DataTable InfoToDt(string str)
    {
        return null; 
    }
    //public void GetFuWuqiData()
    //{
    //    ReadCSV();
    //}  
}
