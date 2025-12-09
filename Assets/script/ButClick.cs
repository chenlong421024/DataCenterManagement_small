using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButClick : MonoBehaviour
{
    static int index = 1;
    LookAt la = new LookAt();
    Transform tr;
    Vector3 v_old;
    Vector3 v_new;
    void Start()
    {
        v_old = v_new = Camera.main.transform.position;
        tr = Camera.main.transform;
    }

    // Update is called once per frame
    public void BtnListClick()
    {
       index *= -1;
        // Button bt = this.GetComponent<Button>();
        // Text t= transform.Find("Text").GetComponent<Text>();
        Text t = GameObject.Find("Canvas").transform.Find("ButtonServerList").Find("Text").GetComponent<Text>();
        t.text = (index == 1 ? "取消显示" : "显示列表");
        GameObject.Find("Canvas").transform.Find("ScrollView").gameObject.SetActive(index == 1 ? true : false );
        if (index == 1)//jia zai shu ju 
        {
            int row = DoCSVSQL.Instance.GetDt().Rows.Count;
            if (row <= 0)
            {
                DoCSVSQL.Instance.ReadCSV();
            }
        }
        if (this.name.Contains("panel"))//判断是点击的是按钮还是panel
        {
            if (this.name == "panel_0")//标题
                return;
            Text t_jigui = this.transform.Find("text1").GetComponent<Text>();
            Text t_u = this.transform.Find("text2").GetComponent<Text>();
            Vector3 vc=la.getPos(t_jigui.text,t_u.text);
            //tr.LookAt(vc);
            v_old = tr.transform.position;
            v_new = vc;
            v_new.x += 1;
            //Debug.Log(Vector3.Distance(v_new, v_old));
            Messenger.Broadcast<PointsMes>(GameEvent.MoveCreama,new PointsMes(v_new,v_old));
        }
    }
    /// <summary>
    /// 添加服务器 点击后弹出info面板 自动添加序号
    /// </summary>
    public void BtnAddServerMac()
    {
        //Debug.Log("BtnAddServerMac");
        //发送弹出消息
        int row = DoCSVSQL.Instance.GetDt().Rows.Count+1;
        Messenger.Broadcast<ShowMes>(GameEvent.Show, new ShowMes(UIMenu.PanelInfo, row));
    }
    /// <summary>
    /// 关闭添加面板
    /// </summary>
    public void BtnPanelInfoClose()
    {
        Messenger.Broadcast<UIMenu>(GameEvent.Hide, UIMenu.PanelInfo);
    }
    /*
     * public float Interval = 0.5f;
     private float firstClicked = 0;
    private float secondClicked = 0;


    public void OnDoubleClicked()
    {
        secondClicked = Time.realtimeSinceStartup;

        if (secondClicked - firstClicked < Interval)
        {
            print("clicked");
        }
        else
        {
            firstClicked = secondClicked;
        }
    }
      */

}
