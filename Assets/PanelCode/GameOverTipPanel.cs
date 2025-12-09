using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOverTipPanel : PanelBase
{
    private Text[] textName;
    private Text[] textTime;
    private Text[] textPaiMing;
    private Transform content;
    private GameObject roomPrefab;
    //private Button closeBtn;
    //private Button newBtn;
    private Button quickJoinBtn;
    private Object[] objRoomPrefabs;
    IEnumerator coroutine;
    //int timeRemain;
    #region 生命周期
    /// <summary> 初始化 </summary>
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "GameOverTipPanel";
        layer = PanelLayer.Panel;
        textName = new Text[4];
        textTime = new Text[4];
        textPaiMing = new Text[4];
        coroutine = RecordTime();
       // timeRemain = 5;
        //PlayerDataMgr.GetInstance().playerData[0].state = PlayStatueEnum.GameOver;
    }
    public override void OnShowing()
    {
        base.OnShowing();
        //获取Transform
        Transform skinTrans = skin.transform;
        for (int i = 1; i <= 4; i++)
        {
            string panel = "Panel" + i;
            string textN = panel + "/TextName";
            string textT = panel + "/TextTime";
            string textP = panel + "/TextScore";
            textName[i - 1] = skinTrans.Find(textN).GetComponent<Text>();
            textTime[i - 1] = skinTrans.Find(textT).GetComponent<Text>();
            textPaiMing[i - 1] = skinTrans.Find(textP).GetComponent<Text>();
        }
        SortPaiMing();
        StartCoroutine(coroutine);
    }
    void SortPaiMing()
    {
        ProtocolBytes proto = (ProtocolBytes)this.args[0];
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int count = proto.GetInt(start, ref start);
        int[] pm = new int[4] { -1, -1, -1, -1 };
        string[] tt = new string[4] { "", "", "", "" };
        string[] tn = new string[4] { "", "", "", "" };
        for (int i = 0; i < count; i++)
        {
            string id = proto.GetString(start, ref start);
            tn[i] = proto.GetString(start, ref start);
            tt[i] = proto.GetFloat(start, ref start).ToString();
            pm[i] = proto.GetInt(start, ref start);
        }
        for (int i = 0, j = 1; i < count; i++)//pai xu
        {
            int index = pm[i];
            if (index == -1)
            {
                textName[count - j].text = tn[i];
                textTime[count - j].text = tt[i];
                textPaiMing[count - j].text = pm[i].ToString();
                j++;
            }
            else
            {
                index = index - 1;
                textName[index].text = tn[i];
                textTime[index].text = tt[i];
                textPaiMing[index].text = pm[i].ToString();
            }
        }
        for (int i = count; i < 4; i++)
        {
            textName[i].text = string.Empty;
            textTime[i].text = string.Empty;
            textPaiMing[i].text = string.Empty;
        }
    }
    IEnumerator RecordTime()
    {
        yield return new WaitForSeconds(8f);
        PanelMgr.instance.OpenPanel<RoomPanel>("RoomPanel");
        Close();
    }
    #endregion
    public override void OnEsc()
    {
        Close();
    }
}