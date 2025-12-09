using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.ComponentModel;

public class RegPanel : PanelBase
{
    private InputField idInput;
    private InputField pwInput;
    private InputField pwReInput;
    private InputField phoneInput;
    private InputField validCodeInput;

    private Image idImg;
    private Image pwImg;
    private Image pwReImg;
    private Image phoneImg;
    private Image validCodeImg;


    private Button regBtn;
    private Button getValidCode;
    private Text getValidCodeText;
    //验证码发送服务端
    private int validCode;
    IEnumerator coroutine;
    float step;
    int textInt;
    int textConst = 30;
    #region 生命周期
    //初始化
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "RegPanel";
        layer = PanelLayer.Panel;
        PlayerDataMgr.GetInstance().playerData[0].state = PlayStatueEnum.None;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        step = 6.0f / 180f;
        textInt = 30;
        Transform skinTrans = skin.transform;
        idInput = skinTrans.Find("IDInput").GetComponent<InputField>();
        pwInput = skinTrans.Find("PWInput").GetComponent<InputField>();
        pwReInput = skinTrans.Find("RepInput").GetComponent<InputField>();
        phoneInput = skinTrans.Find("PhoneNumInput").GetComponent<InputField>();
        validCodeInput = skinTrans.Find("ValidCodeInput").GetComponent<InputField>();

        idImg = skinTrans.Find("IDInput/ImageCheck").GetComponent<Image>();
        pwImg = skinTrans.Find("PWInput/ImageCheck").GetComponent<Image>();
        pwReImg = skinTrans.Find("RepInput/ImageCheck").GetComponent<Image>();
        phoneImg = skinTrans.Find("PhoneNumInput/ImageCheck").GetComponent<Image>();
        validCodeImg = skinTrans.Find("ValidCodeInput/ImageCheck").GetComponent<Image>();


        regBtn = skinTrans.Find("RegBtn").GetComponent<Button>();
        getValidCode = skinTrans.Find("GetCheckCodeBtn").GetComponent<Button>();
        getValidCodeText = skinTrans.Find("GetCheckCodeBtn/Text").GetComponent<Text>();

        regBtn.onClick.AddListener(OnRegClick);
        getValidCode.onClick.AddListener(OnGetValidCode);

        idInput.onEndEdit.AddListener(OnEndEditID);
        pwInput.onEndEdit.AddListener(OnEndEditPW);
        pwReInput.onEndEdit.AddListener(OnEndEditPWR);

        phoneInput.onEndEdit.AddListener(OnEndEditPhone);
        validCodeInput.onEndEdit.AddListener(OnEndEditValid);
        validCodeInput.interactable = false;
        coroutine = RecordTime();
    }
    #endregion


    public void OnCloseClick()
    {
        PanelMgr.instance.OpenPanel<LoginPanel>("");
        Close();
    }

    public void OnRegClick()
    {
        if(!InputValidCheck())
            return;
        //用户名密码为空
          //连接服务器
        if (NetMgr.srvConn.status != Connection.Status.Connected)
        {
            //string host = "127.0.0.1";
            //int port = 1234;
            string host = ConfigInfo.Instance.IP;
            if (host == string.Empty)
                return;
            int port = ConfigInfo.Instance.Port;
            NetMgr.srvConn.proto = new ProtocolBytes();
            if (!NetMgr.srvConn.Connect(host, port))
                PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "连接服务器失败!");
        }
        //发送
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Register");
        protocol.AddString(idInput.text);
        protocol.AddString(pwInput.text);
        protocol.AddString(phoneInput.text);
        Debug.Log("发送 " + protocol.GetDesc());
        NetMgr.srvConn.Send(protocol, OnRegBack);
    }

    public void OnRegBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        if (ret == 0)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "LoginPanel", "注册成功!");
            //PanelMgr.instance.OpenPanel<LoginPanel>("");
            Close();
        }
        else
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "该手机号码已注册\n注册失败!");
        }
    }
    public void OnGetValidCode()
    {
        if (!Check.CheckPhoneNum(phoneInput.text))
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "请输入正确的手机号");
            return;
        }
        if (!validCodeInput.interactable)
            validCodeInput.interactable = true;
        validCodeInput.enabled = true;
        validCodeInput.text = "";
        validCode = UnityEngine.Random.Range(1000, 9999);
        Debug.Log(validCode);
        if (NetMgr.srvConn.status != Connection.Status.Connected)
        {
            //string host = "127.0.0.1";
            //int port = 1234;
            string host = ConfigInfo.Instance.IP;
            if (host == string.Empty)
                return;
            int port = ConfigInfo.Instance.Port;
            NetMgr.srvConn.proto = new ProtocolBytes();
            if (!NetMgr.srvConn.Connect(host, port))
            { 
                PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "连接服务器失败!");
                return;
            }
        }
        //发送
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("ValidCode");
        protocol.AddInt(1);
        protocol.AddString(phoneInput.text);
        protocol.AddString(validCode.ToString());
        Debug.Log("发送 " + protocol.GetDesc());
        NetMgr.srvConn.Send(protocol, OnValidCodeBack);

        getValidCode.image.fillAmount = 0;
        getValidCode.enabled = false;
        validCodeImg.fillAmount = 0;
        step /= 2;
        textConst *= 2;
        textInt = textConst;
        StartCoroutine(coroutine);
    }
    public void OnValidCodeBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        if (ret == 0)
        {
            return;
        }
        else
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "网络延时或电话号码已被注册！");
            textInt = 0;
        }
    }
    public bool InputValidCheck()
    {
        //string[] strPar = new string[2];
        //strPar[0] = skinPath;
        if (idInput.text == "" || pwInput.text == ""|| validCodeInput.text=="")
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "用户名、密码或验证码不能为空!");
            return false ;
        }
        //两次密码不同
        if (pwInput.text != pwReInput.text)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "两次输入的密码不同！");
            return false;
        }
        if (!Check.CheckUserName(idInput.text))
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "输入用户名不规范\n1、字母开头\n2、只能有字母、数字、下划线\n3、6 - 18位长度");
            return false;
        }

        if (!Check.CheckPassWold(pwInput.text))
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "密码输入错误\n1、字母开头\n2、只能有字母或数字\n3、6 - 18位长度");
            return false;
        }
        if (!(validCodeInput.text == validCode.ToString() || validCodeImg.fillAmount == 1))
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", skinPath, "验证码输入错误");
            return false;
        }
        return true;
    }
    public void OnEndEditID(string str)
    {
        if (Check.CheckUserName(str))
        {
            idImg.fillAmount = 1;
        }
        else
        {
            idImg.fillAmount = 0;
        }
    }
    public void OnEndEditPW(string str)
    {
        if (Check.CheckPassWold(str))
        {
            pwImg.fillAmount = 1;
            if (pwReInput.text == str)
            {
                pwReImg.fillAmount = 1;
            }
        }
        else
        {
            pwImg.fillAmount = 0;
        }
    }
    public void OnEndEditPWR(string str)
    {
        if (Check.CheckPassWold(str)&& str==pwInput.text)
        {
            pwReImg.fillAmount = 1;
        }
        else
        {
            pwReImg.fillAmount = 0;
        }
    }
    public void OnEndEditPhone(string str)
    {
        if (Check.CheckPhoneNum(str))
        {
            phoneImg.fillAmount = 1;
        }
        else
        {
            phoneImg.fillAmount = 0;
        }
    }
    public void OnEndEditValid(string str)
    {
        if (Check.CheckValidCode(str) && validCodeInput.text == validCode.ToString())
        {
            validCodeImg.fillAmount = 1;
            validCodeInput.enabled = false;
        }
        else
        {
            validCodeImg.fillAmount = 0;
        }
    }
    IEnumerator RecordTime()
    {
        while (true)
        {
            getValidCode.image.fillAmount += step;
            textInt--;
            getValidCodeText.text = textInt.ToString();
            if (textInt <= 0)
            {
                getValidCode.image.fillAmount = 1;
                getValidCode.enabled = true;
                validCode = 0;
                getValidCodeText.text = "获取短信\n验证码";
                textInt = textConst;
                StopCoroutine(coroutine);
            }
            yield return new WaitForSeconds(1f);
        }
    }
    public override void OnClosing()
    {
        if (regBtn.onClick != null)
        {
            regBtn.onClick.RemoveAllListeners();
        }
        if (getValidCode.onClick!=null)
        {
            getValidCode.onClick.RemoveAllListeners();
        }
        if (idInput.onEndEdit != null)
        {
            idInput.onEndEdit.RemoveAllListeners();
        }
        if (pwInput.onEndEdit != null)
        {
            pwInput.onEndEdit.RemoveAllListeners();
        }
        if (pwReInput.onEndEdit != null)
        {
            pwReInput.onEndEdit.RemoveAllListeners();
        }
        if (phoneInput.onEndEdit != null)
        {
            phoneInput.onEndEdit.RemoveAllListeners();
        }
        if (validCodeInput.onEndEdit != null)
        {
            validCodeInput.onEndEdit.RemoveAllListeners();
        }
    }

    public override void OnEsc()
    {
        Close();
    }
}