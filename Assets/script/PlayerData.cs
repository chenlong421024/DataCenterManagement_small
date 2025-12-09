/// <summary>
/// 
/// </summary>
public class PlayerData
{
    //id、连接、玩家数据
    public  string name;
    public  string phone ;
    public  float bestRecord;
    public  int ranking;
    public  int roomNum;
    public  bool isOwner;
    public  int seat;
    public GameModeEnum gamemode;
    public PlayStatueEnum state;
    //构造函数，给id和conn赋值
    public PlayerData()
    {
        name = string.Empty;
        phone = string.Empty;
        bestRecord = 1000000;
        ranking = -1;
        roomNum = -1;
        isOwner = false;
        seat = -1;
        gamemode = GameModeEnum.none;
        state = PlayStatueEnum.None;
    }
    public void Init(string name, string phone, float bestRecord, int ranking)
    {
        this.name = name;
        this.phone = phone;
        this.bestRecord = bestRecord;
        this.ranking = ranking;
        state = PlayStatueEnum.Login;
    }
    public void Init(string name)
    {
        this.name = name;
        
    }
    public void Clear()
    {
        name = string.Empty;
        phone = string.Empty;
        bestRecord = 1000000;
        ranking = -1;
        roomNum = -1;
        isOwner = false;
        seat = -1;
        gamemode = GameModeEnum.none;
        state = PlayStatueEnum.None;
    }
}