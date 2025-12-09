using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
public class StartPanel : PanelBase
{

    private Button buttonSingleMode;
    private Button buttonOnlineMode;
    private Button buttonAboutGame;
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "StartPanel";
        layer = PanelLayer.Panel;
        PlayerDataMgr.GetInstance().playerData[0].Clear();//.state = PlayStatueEnum.None;
    }
    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        buttonSingleMode = skinTrans.Find(GameDefine.ButtonSingleMode).GetComponent<Button>() as Button;
        buttonOnlineMode = skinTrans.Find(GameDefine.ButtonOnlineMode).GetComponent<Button>() as Button;
        buttonAboutGame = skinTrans.Find(GameDefine.ButtonAboutGame).GetComponent<Button>() as Button;

        buttonSingleMode.onClick.AddListener(ButtonSingleModeClick);
        buttonOnlineMode.onClick.AddListener(ButtonOnlineModeClick);
        buttonAboutGame.onClick.AddListener(ButtonAboutClick);
    }

    private void ButtonSingleModeClick()
    {
        DoCSVSQL.Instance.ISOnline = false;
        Messenger.Broadcast(GameEvent.LoadFuWuQi);
        Close();
    }
    private void ButtonOnlineModeClick()
    {
        Close();
        PanelMgr.instance.OpenPanel<LoginPanel>("");
    }
    private void ButtonAboutClick()
    {
        PanelMgr.instance.OpenPanel<InfoPanel>("InfoPanel", skinPath);
    }
    public override void OnClosing()
    {
        if(buttonSingleMode.onClick!=null)
        buttonSingleMode.onClick.RemoveAllListeners();
        buttonOnlineMode.onClick.RemoveListener(ButtonOnlineModeClick);
        buttonAboutGame.onClick.RemoveListener(ButtonAboutClick);
    }

    public override void OnEsc()
    {
        Application.Quit();
    }
}
