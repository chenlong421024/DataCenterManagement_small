using System.Collections;
using System.Collections.Generic;


public class Dict
{
    private static readonly Dict instance = null;
   public static Dict Instance
    {
        get
        {
            return instance;
        }
    }
    //private static DataTable dt = null;
    static Dict()
    {
        instance = new Dict();
        init();
    }
    private Dict()
    {

    }

    private static Dictionary<int, string> uiDic;
    private static Dictionary<int, string> sqlDic;
    private static Dictionary<int , string> PanelTitleStrintDic;
    private static void  init()
    {
        uiDic = new Dictionary<int, string>();//panel 
        sqlDic = new Dictionary<int, string>();//dt
        PanelTitleStrintDic= new  Dictionary<int, string>();//dt
        int i = 0;
        int j = 1;
        uiDic.Add(i++, "jigui"); sqlDic.Add(i,StringConst.jiguiWhich);//1
        PanelTitleStrintDic.Add(j++, StringConst.jiguiWhich);
        uiDic.Add(i++, "jiguiHighPos"); sqlDic.Add(i, StringConst.jiguiHighPos);//2
        PanelTitleStrintDic.Add(j++, StringConst.jiguiHighPos);
        uiDic.Add(i++, "serverName"); sqlDic.Add(i,StringConst.serverName);//3
        PanelTitleStrintDic.Add(j++, StringConst.serverName);

        uiDic.Add(i++, "IP"); sqlDic.Add(i, StringConst.IP);//4
        PanelTitleStrintDic.Add(j++, StringConst.IP);

        uiDic.Add(i++, "isOpen"); sqlDic.Add(i, StringConst.isOpen);//5
        PanelTitleStrintDic.Add(j++, StringConst.isOpen);

        uiDic.Add(i++, "serverBrand"); sqlDic.Add(i, StringConst.serverBrand);
        uiDic.Add(i++, "model"); sqlDic.Add(i, StringConst.model);
        uiDic.Add(i++, "years"); sqlDic.Add(i, StringConst.years);
        uiDic.Add(i++, "CPU"); sqlDic.Add(i, StringConst.CPU);
        uiDic.Add(i++, "RAM"); sqlDic.Add(i, StringConst.RAM);
        uiDic.Add(i++, "disk"); sqlDic.Add(i, StringConst.disk);//10
        uiDic.Add(i++, "Department"); sqlDic.Add(i, StringConst.Department);
        uiDic.Add(i++, "ContactPerson"); sqlDic.Add(i, StringConst.ContactPerson);
        uiDic.Add(i++, "tel"); sqlDic.Add(i, StringConst.Tel);
        uiDic.Add(i++, "remark"); sqlDic.Add(i, StringConst.remark);//15  //6
        PanelTitleStrintDic.Add(j++, StringConst.remark);


    }
    public Dictionary<int, string> UIDic
    {
        get
        {
            return uiDic;
        }
    }
    public Dictionary<int, string> SQLDic
    {
        get
        {
            return sqlDic;
        }
    }
    public Dictionary<int, string> panelTitleStrintDic
    {
        get
        {
            return PanelTitleStrintDic;
        }
    }
}
