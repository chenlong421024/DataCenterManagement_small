using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TipPanel : PanelBase
{
    private Text text;
    private Button btnOK;
    private Button btnCancel;
    string str = "";

    #region 生命周期

    //初始化
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "TipPanel";
        layer = PanelLayer.Tips;
        layer = PanelLayer.Panel;
        //参数 args[1]代表提示的内容
        if (args.Length == 1)
        {
            str = (string)args[0];
            Debug.Log(args[0]);
        }
        else if (args.Length == 2)
        {
            str = (string)args[0];
            Debug.Log(args[0]);
            Debug.Log(args[1]);
        }
    }

    //显示之前
    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        //文字
        text = skinTrans.Find("Text").GetComponent<Text>();
        text.text = str;
        //关闭按钮
        btnOK = skinTrans.Find("BtnOk").GetComponent<Button>();
        btnCancel = skinTrans.Find("Cancel").GetComponent<Button>();
        //tina jia shang shngchu shijian
        btnOK.onClick.AddListener(OnBtnClickOK);
        btnCancel.onClick.AddListener(OnBtnClickCancel);
    }
    #endregion

    //按下“知道了”按钮的事件
    public void OnBtnClickOK()
    {
        //发送ok 事件
        if (this.args.Length>1 && this.args[1].ToString() == "btnok")
            Messenger.Broadcast(GameEvent.BtnDeleteOk);
        Close();
        
        //return true;
    }
    public void OnBtnClickCancel()
    {
        Close();
        //return true;
    }
    public override void OnEsc()
    {
        Close();
    }
}