
public class GameDefine
{
    public const string StartScene = "Scene/Start";
    public const string SingleModeScene = "Scene/SingleModeGaming";
    public const string OnlineModeScene = "Scene/OnlineModeGaming";
    public const string ButtonSingleMode = "ButtonSingleMode";
    public const string ButtonOnlineMode = "ButtonOnlineMode";
    public const string ButtonAboutGame = "ButtonAboutGame";

    public const string ButtonInitMoFang = "ButtonInitMoFang";
    public const string ButtonHomeMoFang = "ButtonHomeMoFang";
    public const string TextStringTime = "Panel/Timepanel/TextStringTime";
    public const string TextPlayInfo = "Panel/Timepanel/TextPlayInfo";
    public const string TextDaoJiShiTime = "Panel/Timepanel/TextDaoJiShiTime";

    //public const string ButtonInitMoFang = "Canvas/ButtonInitMoFang";
    public const string TextInitMoFang = "ButtonInitMoFang/TextInitMoFang";

    public const string QuadRed = "QuadRed";
    public const string QuadYellow = "QuadYellow";
    public const string QuadGreen = "QuadGreen";
    public const string QuadOrange = "QuadOrange";
    public const string QuadWhite = "QuadWhite";
    public const string QuadBlue = "QuadBlue";

    public const string Restore = "复原";
    public const string Disrupt = "打乱";

}
/// <summary>
/// 发送初始化姓名消息
/// </summary>
public class MsgName
{
    public int index;
    public string phone;
    public string name;
    public MsgName(int index, string phone, string name)
    {
        this.index = index;
        this.phone = phone;
        this.name = name;
    }
}
/// <summary>
/// 发送旋转消息
/// </summary>
public class MsgRotate
{
    public string phone;
    public int ratate;
    public MsgRotate(string phone, int ratate)
    {
        this.phone = phone;
        this.ratate = ratate;
    }
}
public enum GameEvent
{
    None = -1,
    MoveCreama,
    OpenDoor,
    Show,
    Hide,
    LoadData,
    LoadFuWuQi,
    BtnDeleteOk,
    DeletePanel,
    AddPanel,
    ReFresh,
    LoadPanelInfo
}
public enum PlayStatueEnum
{
    None = -1,
    Logout,
    Login,
    InRoom,
    Ready,
    Fight,
    GameOver,
    Complete,
    UnComplete
}
public enum GameModeEnum
{
    none = -1,
    single,
    online
}
