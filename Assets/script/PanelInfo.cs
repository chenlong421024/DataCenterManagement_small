using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PanelInfo : MonoBehaviour
{
    int NUM = 15;

    // Start is called before the first frame update
    InputField[] tInfo = new InputField[15];//0 wei yong
    Dropdown drA;
    Dropdown drNum;

    void Start()
    {

        Transform tr = GameObject.Find("Canvas/PanelInfo/ScrollView/Viewport/Content/").transform;
        //Debug.Log("InitInfo   "+tr.name);
        if (tr)
        {
            drA = (Dropdown)tr.Find("jigui/DropdownA").GetComponent<Dropdown>();
            GameObject goP = tr.Find("jigui/DropdownA").gameObject;
            Transform t1r = tr.Find("jigui/DropdownA");
            //drA = (Dropdown)t1r.gameObject;
            drA.ClearOptions();
            drA.AddOptions(new List<string> { "A", "B", "C", "D", "E", "F", "G", "H" });
            drNum = (Dropdown)tr.Find("jigui/DropdownNUM").GetComponent<Dropdown>();
            drNum.ClearOptions();
            drNum.AddOptions(new List<string> { "1", "2", "3", "4", "5", "6", "7" });

            string str = null;
            for(int i = 1; i < NUM ; i++)//15列 不带序号
            {
                str = Dict.Instance.UIDic[i] + "/InputField";
                tInfo[i] = (InputField)tr.Find(str).GetComponent<InputField>();
            }
        }
        Messenger.AddListener(GameEvent.BtnDeleteOk, Delete);//确认删除
        Messenger.AddListener<int>(GameEvent.LoadPanelInfo, LoadInfo);//确认删除
        // Messenger.Broadcast<int>(GameEvent.LoadPanelInfo, index);
       // Messenger.AddListener()
       // Messenger.AddListener(GameEvent.BtnOk, ButtonDelete);
    }

    public void ButtonDelete()
    {
        //PanelMgr.instance.OpenPanel<TipPanel>("", "新增无法删除！", "btnok");//new object[] { "arg1", "arg2" }
        PanelMgr.instance.OpenPanel<TipPanel>("", new object[] { "是否确认删除！", "btnok" });//
    }
    public void Delete()
    {
        Transform tr = GameObject.Find("Canvas/PanelInfo/ScrollView/Viewport/Content/").transform;
        string str = tr.Find("xuhao/TextXuhao").GetComponent<Text>().text;
        Debug.Log(str + "   TextXuhao");
        DataTable dt = DoCSVSQL.Instance.GetDt();
        int index = int.Parse(str);
        if (index <= DoCSVSQL.Instance.GetDt().Rows.Count)
        {
            string jigui = dt.Rows[index-1][StringConst.jiguiWhich].ToString();//
            string jHeighPos = dt.Rows[index-1][StringConst.jiguiHighPos].ToString();
            string str1 = dt.Rows[index - 1][0].ToString() + "_" + jigui + "_" + jHeighPos;
            // 我们可以使用RowFilter来查找要删除的行  
            DataRow rowsToDelete = dt.Rows[index-1];
            // 删除找到的行  
            dt.Rows.Remove(rowsToDelete);  // 这会从DataTable中删除行，但它不会从数据库中删除数据（如果DataTable是从数据库中获取的）  
            DoCSVSQL.Instance.WriteCSV();
        
            Messenger.Broadcast<string >(GameEvent.DeletePanel, str1);
        }
        else
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "新增无法删除！");
        }
    }
    public void ButtonSave()
    {
        //bool isOk=DisplayDialog("保存", "保存成功", "退出编辑", "返回编辑");
        //Debug.Log(isOk.ToString());
        //if (isOk)
        //{
        //}
        //读取 判断  
        //保存成功
        // Messenger.Broadcast<UIMenu>(GameEvent.Hide, UIMenu.PanelInfo);
        //保存到数据库
        //机柜 单元 和服务器名称必填
        //判断是本地还是网络
        //判断数据的格式是否正确
        if (DoCSVSQL.Instance.ISOnline == false)
        {
            //写dt
            Transform tr = GameObject.Find("Canvas/PanelInfo/ScrollView/Viewport/Content/").transform;
            string str = tr.Find("xuhao/TextXuhao").GetComponent<Text>().text;
            Debug.Log(str + "   TextXuhao");
            DataTable dt = DoCSVSQL.Instance.GetDt();
            int index = int.Parse(str);
            bool isParamOk = true;
            //tInfo[1].text  A1 
            //appName = iNIParser.ReadValue("System", "软件名称", "华远");
            string pos = ConfigInfo.Instance.GetPoseArer();//abcdefg
            string posmax = ConfigInfo.Instance.GetPoseArer();//abcdefg
            string umax = ConfigInfo.Instance.GetPoseArer();//abcdefg
            string tipString = "错误代码：";
            do
            {
                //if(tInfo[0].text.Length>2)
                //{
                //    isParamOk = false;
                //    tipString += tInfo[0].text;
                //    break;
                //}
                for (int i = 0; i < 4; i++)//前面5个 必填项目
                {
                    if (IsOkData(tInfo[i].text) == false)
                    {
                        isParamOk = false;
                        tipString += tInfo[0].text;

                        break;
                    }
                }

                    //}
                    //if (isParamOk == false)
                    //{
                    //    break;
                    //}
                    //if (!pos.Contains(tInfo[0].text[0]) || tInfo[0].text.Length <= 1)//超过范围
                    //{
                    //    tipString += tInfo[0].text;

                    //    isParamOk = false;
                    //    break;
                    //}
                    ////tInfo[2].text  1u 
                    //if (tInfo[0].text[0] - 'a' >= 8)
                    //{
                    //    tipString += tInfo[0].text;

                    //    isParamOk = false;
                    //    break;
                    //}
                    //判断错误40u 25u
                 if (!Check.IsNum(tInfo[1].text))
                {
                    tipString += tInfo[1].text;
                    isParamOk = false;
                    break;
                }
                
                if (!Check.IsValidIP(tInfo[3].text))
                {
                    tipString += tInfo[3].text;

                    isParamOk = false;
                    break;
                }
            } while (false );
            
            if (isParamOk == false)
            {
                PanelMgr.instance.OpenPanel<TipPanel>("", "请确认您输入的数据格式是正确的!\n"+ tipString);
                return;
            }
            bool isnew = false;
            //
            //
            tInfo[0].text = drA.options[drA.value].text+drNum.options[drA.value];
            if (index <= DoCSVSQL.Instance.GetDt().Rows.Count)//213 212
            {//update

                for (int i = 1; i < NUM ; i++)
                {
                    dt.Rows[index-1][i] = tInfo[i-1].text;//qu diao xu hao
                }
            }
            else
            {//new
                DataRow dr = dt.NewRow();//这里添加了一行
                dr[0] = index;
                for (int j = 1; j < dt.Columns.Count; j++)//1-15
                {
                    dr[j] = tInfo[j-1].text;//0-14
                }
                dt.Rows.Add(dr);
                isnew = true;
            }
            DoCSVSQL.Instance.WriteCSV();
            //刷新界面
            if (isnew)
                Messenger.Broadcast<int>(GameEvent.AddPanel, index);
            else
            {
                //刷新ReFresh
                Messenger.Broadcast<int>(GameEvent.ReFresh, index);
            }
        }
        else
        {
            //写数据库
        }
        
        //本地模式没有保存每次联网自动同步
        //联网 写dt 在写数据库（判断是否为安全写入） 刷新对应的行和对应的服务器


        //string connectionString = "Data Source=(local);Initial Catalog=YourDatabase;Integrated Security=True";
        //string query = "INSERT INTO YourTable (Column1, Column2) VALUES (@value1, @value2)";

        //using (SqlConnection connection = new SqlConnection(connectionString))
        //{
        //    SqlCommand command = new SqlCommand(query, connection);
        //    command.Parameters.AddWithValue("@value1", "Value1");
        //    command.Parameters.AddWithValue("@value2", "Value2");

        //    try
        //    {
        //        connection.Open();
        //        command.ExecuteNonQuery();
        //    }
        //    catch (SqlException e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //}
        PanelMgr.instance.OpenPanel<TipPanel>("", "保存成功!");
        //PanelMgr.instance.OpenPanel<InfoPanel>("TipBtnSave");
        //弹出保存成功
        //发送刷新数据库的通知
    }

    //private bool DisplayDialog(string v1, string v2, string v3, string v4)
    //{
    //    throw new NotImplementedException();
    //}
    bool IsOkData(string str)
    {
        
        if (string.IsNullOrEmpty(str))
        {
            return false;
        }
        
        return true ;
    }
    public void ButtonCancel()
    {
        //隐藏此表
        Messenger.Broadcast<UIMenu>(GameEvent.Hide, UIMenu.PanelInfo);
    }
    //public void Delete

    void LoadInfo(int index)
    {
        //判断index是否是大于最大行 

        GameObject.Find("Canvas/PanelInfo/ScrollView/Viewport/Content/xuhao/TextXuhao").GetComponent<Text>().text = index.ToString();
        if (index <= DoCSVSQL.Instance.GetDt().Rows.Count)//update
        {
            GameObject.Find("Canvas/PanelInfo/ButtonDelete").GetComponent<Button>().interactable = true;
            //NUM=DoCSVSQL.Instance.GetDt().Columns.Count;
            for (int i = 0; i < DoCSVSQL.Instance.GetDt().Columns.Count - 1; i++)//
            {
                if (tInfo[i] != null)
                {
                    //Debug.Log(Dict.Instance.SQLDic[i+1]);
                    tInfo[i].text = DoCSVSQL.Instance.GetDt().Rows[index - 1][Dict.Instance.SQLDic[i + 1]].ToString();
                }
            }
            drA.value = tInfo[0].text[0] - 'A';
            drNum.value = tInfo[0].text[1] - '0';
        }
        else//new
        {
            GameObject.Find("Canvas/PanelInfo/ButtonDelete").GetComponent<Button>().interactable = false;
            drA.value =0;
            drNum.value = 0;
            for (int i = 0; i < NUM ; i++)
            {
                if (tInfo[i] != null)
                {
                    //Debug.Log(Dict.Instance.SQLDic[i+1]);
                    tInfo[i].text = "";
                }
            }
        }

    }
}
