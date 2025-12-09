using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ListLayout : MonoBehaviour
{
    // Start is called before the first frame update
    Transform tra;
    Transform FuWuQi;
    float oldwidth;
    float newwidth;
    float height;
    int col;
    int row;
    DataTable dt;
    float ColumnsLength;
    int COLUMNS = 7;
    public Vector3 firstPos;//8.6 -1.26 -2.52      jigui x 1.227  z 0.815
    public Vector3 lastPos;//-8.64 0.69 2.64
    float jgrowSize;
    float jgcolSize;
    private static Dictionary<int, string> Dic_id_fuwuqi;
    //IEnumerator coroutine;
    private void Awake()
    {
        ColumnsLength = 0;
    }
    void Start()
    {
        firstPos = new Vector3(-8.64f,-1.26f, -2.52f);//a1-1u   - - -
        lastPos = new Vector3(8.6f, 0.69f, 2.64f);//h7-40u + + +
        jgrowSize = 1.227f;//机柜的长
        jgcolSize = 0.815f;//机柜的宽度

        tra = GameObject.Find("Canvas").transform.Find("ScrollView/Viewport/Content");
        FuWuQi = GameObject.Find("FuWuQi").transform;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GameObject.Find("Canvas").GetComponent<RectTransform>());
        //监听获得数据的信息是单击还是联网
        Messenger.AddListener(GameEvent.LoadFuWuQi, GetFuWuqiData);
        //Debug.Log("Messenger.AddListener(GameEvent.LoadFuWuQi, GetFuWuqiData)");
        //coroutine = LoadPanel();
        Messenger.AddListener<string>(GameEvent.DeletePanel, DeletePanel);
        Messenger.AddListener<int>(GameEvent.AddPanel, AddPanel);
        Messenger.AddListener<int>(GameEvent.ReFresh, ReFresh);
        Dic_id_fuwuqi = new Dictionary<int, string>();
    }
    public void ReFresh(int index)
    {
        string str = "panel" + "_" + index.ToString();
        Transform panelText = tra.Find(str);
        for(int i=1;i<6;i++)//text0 xuhao
        {
            string tstr = "text"+i ;
            panelText.transform.Find(tstr).GetComponent<Text>().text= dt.Rows[index-1][Dict.Instance.panelTitleStrintDic[i]].ToString();
        }
        ReLaout();
        LayoutRebuilder.ForceRebuildLayoutImmediate(tra.GetComponent<RectTransform>());
        ReFreshFuWuQi(index - 1);
    }

    //删除panel_x
    public void DeletePanel(string str)
    {
        //string jigui = dt.Rows[i-1][StringConst.jiguiWhich].ToString();
        //string jHeighPos = dt.Rows[i-1][StringConst.jiguiHighPos].ToString();
        string strName="fwq" + "_" +str;
        int i = int.Parse(str.Split('_')[0]);
        GameObject fwq = GameObject.Find(strName).gameObject;
        //GameObject fwq = GameObject.FindGameObjectWithTag("fwq"+i);
        GameObject panelText = tra.Find("panel_" +i).gameObject;
        if (panelText != null)
        {
            Destroy(panelText);
        }
        
        if (fwq != null)
        {
            Destroy(fwq);
        }
        Dic_id_fuwuqi.Remove(i);

    }
    public void AddPanel(int index)
    {
        GameObject panelText = Instantiate(Resources.Load("Prefabs/PanelTextSerInfo")) as GameObject;
        panelText.name = "panel" + "_" + index.ToString();
        Text t = panelText.transform.Find("Text").GetComponent<Text>();
        panelText.transform.SetParent(tra);//ScrollView/Viewport/Content
        t.text = "";
        //
        //oldwidth=tra.GetComponent<RectTransform>().rect.width;
        //height = panelText.GetComponent<RectTransform>().rect.height;
        //Debug.Log(oldwidth + "  -----------  " + height);
        for (int j = 0; j < COLUMNS; j++)//COLUMNS = 7
        {
             SetText(j, panelText, dt.Rows[index - 1][j].ToString());
        }
        ReLaout();
        LayoutRebuilder.ForceRebuildLayoutImmediate(tra.GetComponent<RectTransform>());
        AddFuWuQi(index-1);
    }


    //获取数据
    public void GetFuWuqiData()
    {
        //在这里利用消息
        Debug.Log("GetFuWuqiData()");
        dt = DoCSVSQL.Instance.GetDt();
        row = dt.Rows.Count;
        if (row <= 0)
        {
            DoCSVSQL.Instance.ReadCSV();
            LoadPanel(); ;//xie cheng
        }
        else
        {
            LoadPanel();
        }
        //--------消息--------

        ReLaout();
        LoadFuWuQi();
    }
    //IEnumerator LoadPanel()
    void LoadPanel()
    {
        //StringBuilder sb = new StringBuilder();
        Debug.Log("LoadPanel on");
        dt = DoCSVSQL.Instance.GetDt();
        row = dt.Rows.Count;
        col = dt.Columns.Count;

        for (int i = 0; i <= row; i++)
        {
            GameObject panelText = Instantiate(Resources.Load("Prefabs/PanelTextSerInfo")) as GameObject;
            //标题
            //if (i == 0)
            //    panelText.name = "title";
            //else
            panelText.name = "panel" + "_" + i.ToString();
            Text t = panelText.transform.Find("Text").GetComponent<Text>();
            panelText.transform.SetParent(tra);//ScrollView/Viewport/Content
            t.text = "";
            //
            //oldwidth=tra.GetComponent<RectTransform>().rect.width;
            //height = panelText.GetComponent<RectTransform>().rect.height;
            //Debug.Log(oldwidth + "  -----------  " + height);
            for (int j = 0; j < COLUMNS; j++)//COLUMNS = 7
            {
                if (i == 0)//标题
                {
                    SetText(j, panelText, dt.Columns[j].ColumnName);//只显示前面7列
                    ColumnsLength += dt.Columns[j].ColumnName.Length;
                }
                else
                {
                    SetText(j, panelText, dt.Rows[i - 1][j].ToString());
                }
            }

        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(tra.GetComponent<RectTransform>());
        Debug.Log("LoadPanel off");
        //yield break;
    }
    /// <summary>
    /// 设置text 字体
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="panelText"></param>
    /// <param name="str"></param>
    void SetText(int j, GameObject panelText, string str)
    {
        GameObject text = new GameObject();
        text.transform.SetParent(panelText.transform);
        Text t=text.AddComponent<Text>();
        //t.rectTransform.sizeDelta = new Vector2(t.flexibleWidth, 30);
        //t.alignment=
        text.name = "text" + j;
        var style = text.GetComponent<Text>();
        style.text = str;
        style.fontSize = 18;
        style.fontStyle = FontStyle.Normal;
        Font ft = Resources.GetBuiltinResource<Font>("Arial.ttf");
        style.font = ft;
        style.font.material.color = Color.red;
        style.color = Color.black;
    }
    // Update is called once per frame
    void Update()
    {
        //float w = panelText.GetComponent<RectTransform>().rect.width / col;
        //if (Input.GetMouseButtonDown(0))
        //{//计算摄像机和机柜的距离
        //    newwidth = tra.GetComponent<RectTransform>().rect.width;
        //    if (newwidth != oldwidth)
        //    {
        //        oldwidth = newwidth;
        //       // ReLaout();
        //    }
        //}
    }
    /// <summary>
    /// 调整显示面板的显示位置
    /// </summary>
    void ReLaout()
    {
        Debug.Log("ReLaout() on");
        for (int i=0;i<= dt.Rows.Count; i++)
        {
            string str = "panel" + "_" + i.ToString();
            GameObject c = tra.Find(str).gameObject;
            //GameObject t = c.transform.Find();
            for (int j = 0; j < COLUMNS; j++)
            {
                if (dt.Columns[j].ColumnName == StringConst.serverName || dt.Columns[j].ColumnName == StringConst.remark)
                {
                    string tstr = "text" + j;
                    Transform t = c.transform.Find(tstr);

                    t.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 30);
                }
                if (dt.Columns[j].ColumnName == StringConst.IP )
                {
                    string tstr = "text" + j;
                    Transform t = c.transform.Find(tstr);
                    t.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 30);
                }
            }
            //Debug.Log(dt.Columns[j].ColumnName.Length);
        }
        Debug.Log("ReLaout() off");
    }
    void LoadFuWuQi()
    {
        Debug.Log("LoadFuWuQi  on");
        string jigui="";
        string jHeighPos="";
        int i_jigui = 0;
        int i_info = 0;//info
        int i_ip = 0;//ip
        int i_jHeighPos = 0;

        for (int j = 0; j < COLUMNS; j++)
        {
            if (dt.Columns[j].ColumnName == StringConst.jiguiWhich|| dt.Columns[j].ColumnName=="jigui")
            {
                i_jigui = j;
            }
            if (dt.Columns[j].ColumnName == StringConst.jiguiHighPos || dt.Columns[j].ColumnName == "pos")
            {
                i_jHeighPos = j;
            }
            if (dt.Columns[j].ColumnName == StringConst.serverName || dt.Columns[j].ColumnName == "name")//info
            {
                i_info = j;
            }
            if (dt.Columns[j].ColumnName == StringConst.IP || dt.Columns[j].ColumnName == "ip")//ip
            {
                i_ip = j;
            }
        }
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            jigui = dt.Rows[i][i_jigui].ToString();
            jHeighPos = dt.Rows[i][i_jHeighPos].ToString();
            GameObject fwq = Instantiate(Resources.Load("Prefabs/fuwuqi")) as GameObject;
            fwq.name = "fwq" +"_"+ dt.Rows[i][0].ToString() + "_" + jigui + "_" + jHeighPos;
            Dic_id_fuwuqi.Add(i, fwq.name);
            fwq.transform.position = getPos(jigui, jHeighPos);
            fwq.transform.Rotate(Vector3.up * 90 );
            //string tag=
            //fwq.tag = "fwq"+ (i+1);
            //在这里添加服务器信息
            string info= dt.Rows[i][i_info].ToString() + "\n IP:"+dt.Rows[i][i_ip].ToString();
            //transform.Find("backdoor").gameObject.SetActive(false);
            fwq.transform.Find("Text1").GetComponent<TMPro.TextMeshPro>().text = info;
            fwq.transform.Find("Text2").GetComponent<TMPro.TextMeshPro>().text = info;//textmeshpro 位置需要置0 才能正常显示
        }
        Debug.Log("LoadFuWuQi.................off ");
    }
    void ReFreshFuWuQi(int i)
    {
        Debug.Log("LoadFuWuQi  on");
        string jigui = Dic_id_fuwuqi[i].Split('_')[2];
        string jHeighPos = Dic_id_fuwuqi[i].Split('_')[3];
        string name = "fwq" + "_" + dt.Rows[i][0].ToString() + "_" + jigui + "_" + jHeighPos;
        GameObject fwq = GameObject.Find(name);
        string info = dt.Rows[i][Dict.Instance.panelTitleStrintDic[3]].ToString() + "\n IP:" +
            dt.Rows[i][Dict.Instance.panelTitleStrintDic[4]].ToString();
        jigui = dt.Rows[i][StringConst.jiguiWhich].ToString();
        jHeighPos = dt.Rows[i][StringConst.jiguiHighPos].ToString();
        fwq.name = "fwq" + "_" + dt.Rows[i][0].ToString() + "_" + jigui + "_" + jHeighPos;
        fwq.transform.position = getPos(jigui, jHeighPos);
        //fwq.transform.Rotate(Vector3.up * 90);
        //transform.Find("backdoor").gameObject.SetActive(false);
        fwq.transform.Find("Text1").GetComponent<TMPro.TextMeshPro>().text = info;
        fwq.transform.Find("Text2").GetComponent<TMPro.TextMeshPro>().text = info;//textmeshpro 位置需要置0 才能正常显示
        if (Dic_id_fuwuqi.ContainsKey(i))
            Dic_id_fuwuqi[i] = fwq.name;
        else
            Dic_id_fuwuqi.Add(i, fwq.name);
    }
    /// <summary>
    /// 新增
    /// </summary>
    void AddFuWuQi(int i)
    {
        Debug.Log("LoadFuWuQi  on");
        string jigui = dt.Rows[i][StringConst.jiguiWhich].ToString();
        string jHeighPos = dt.Rows[i][StringConst.jiguiHighPos].ToString();
        GameObject fwq = Instantiate(Resources.Load("Prefabs/fuwuqi")) as GameObject;
        fwq.name = "fwq" + "_" + dt.Rows[i][0].ToString() + "_" + jigui + "_" + jHeighPos;
        Dic_id_fuwuqi.Add(i, fwq.name);
        fwq.transform.position = getPos(jigui, jHeighPos);
        fwq.transform.Rotate(Vector3.up * 90);
        //fwq.tag = "fwq" + (i + 1);
        //在这里添加服务器信息
        string info = dt.Rows[i][Dict.Instance.panelTitleStrintDic[3]].ToString() + "\n IP:" +
            dt.Rows[i][Dict.Instance.panelTitleStrintDic[4]].ToString();
        //transform.Find("backdoor").gameObject.SetActive(false);
        fwq.transform.Find("Text1").GetComponent<TMPro.TextMeshPro>().text = info;
        fwq.transform.Find("Text2").GetComponent<TMPro.TextMeshPro>().text = info;//textmeshpro 位置需要置0 才能正常显示
    }
    public Vector3 getPos(string r, string c)//r = A01  c= 20u
    {
        float fx, fy, fz;
        char cx = r[0];//(char)r.Substring(0, 1);//a03
        char cz1='0',cz2='0';
        if (r.Length == 2)
            cz1 = r[1];//(char)r.Substring(0, 1);
        else if (r.Length == 3)
        {
            cz1 = r[1];
            cz2 = r[2];
        }
        int iy=0;
        float xsetp = (lastPos.x-firstPos.x) / 7;
        float ysetp = (lastPos.y-firstPos.y) / 39;
        float zsetp = (lastPos.z-firstPos.z) / 6;
        if (cx >= 'a' && cx <= 'h')
        {
            fx = firstPos.x + (cx - 'a') * xsetp;
        }
        else 
        {
            fx = firstPos.x + (cx - 'A') * xsetp;
        }
        
        string str = c.Split('-')[0];//40u 34 7u
        for (int i=0;i< str.Length; i++)
        {
            if (str[i] == 'U' || str[i] == 'u')
                break;
                iy = iy*10+(str[i]-'0');
        }
        fy = firstPos.y + iy * ysetp;

        char cz = (cz1 != '0') ? cz1 : cz2;
        fz = firstPos.z + (cz - '1') * zsetp;//03

        return new Vector3(fx,fy,fz) ;
    }
}
