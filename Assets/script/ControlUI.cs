using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class ControlUI : MonoBehaviour//canvas
{
    int NUM = 15;
    // Start is called before the first frame update
    GameObject goScrollView;
    GameObject goPanelInfo;
    InputField[] tInfo = new InputField[15];

    //Datatable dt;
    void Start()
    {

        Messenger.AddListener<ShowMes>(GameEvent.Show, Show);
        Messenger.AddListener<UIMenu>(GameEvent.Hide, Hide);
        goScrollView = GameObject.Find("Canvas").transform.Find("ScrollView").gameObject;
        goPanelInfo= GameObject.Find("Canvas").transform.Find("PanelInfo").gameObject;
        //goPanelInfo.SetActive(false );
        //InitInfo();
    }
    void InitInfo()
    {
        //Messenger.Broadcast<UIMenu>(GameEvent.Hide, UIMenu.PanelInfo);
    }
    public GameObject getScrollView()
    {
        return goScrollView;
    }
    public GameObject getPanelInfo()
    {
        return goPanelInfo;
    }
    // Update is called once per frame
    void Show(ShowMes sui)
    {
        //GameObject gameObject=null;
        switch (sui.um)
        {
            case UIMenu.ScrollView:
                {
                     //gameObject=goScrollView;
                    goScrollView.SetActive(true);
                    break;
                }
            case UIMenu.PanelInfo:
                {
                     //gameObject = goPanelInfo;
                    goPanelInfo.SetActive(true);
                    LoadInfo(sui.index);
                    break;
                }
            default :
                {
                    Debug.LogError("void Show(UIMenu ui) error");
                    break;
                }
        }
        //if (gameObject != null)
        //gameObject.SetActive(true);
    }
    void Hide(UIMenu ui)
    {
        GameObject gameObject = null;
        switch (ui)
        {
            case UIMenu.ScrollView:
                {
                    gameObject = goScrollView;
                    break;
                }
            case UIMenu.PanelInfo:
                {
                    gameObject = goPanelInfo;
                    break;
                }
            default:
                {
                    Debug.LogError("void Show(UIMenu ui) error");
                    break;
                }
        }
        if (gameObject != null)
            gameObject.SetActive(false);
    }
    void LoadInfo(int index)
    {
       
        Messenger.Broadcast<int>(GameEvent.LoadPanelInfo, index);
    }
}
