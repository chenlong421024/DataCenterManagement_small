using System;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class DataMgr
{
	MySqlConnection sqlConn;
	float maxTime = 1000000;
	//单例模式
	public static DataMgr instance = null;
	public static DataMgr Instance
	{
		get
		{
			if (instance == null)
				instance = new DataMgr();
			return instance;
		}
	}
	public static bool isCON = false;
	public DataMgr()
	{
		Connect();
	}

	//连接
	public void Connect()
	{
		//数据库
		string connStr = "Database=jifangguanli;Data Source=127.0.0.1;";
		connStr += "User Id=root;Password=chenlong@@123;port=3306;Allow User Variables=True ";
		sqlConn = new MySqlConnection(connStr);
		try
		{
			sqlConn.Open();
			Debug.Log("数据库连接成功！\n");
			//connStr.CharacterSet = "utf8mb4";
			//GetInfo();
			isCON = true;
		}
		catch (Exception e)
		{
			Debug.Log("[DataMgr]Connect " + e.Message);
			isCON = false;
			sqlConn.Close();
			return;
		}
	}

	//判定安全字符串
	public bool IsSafeStr(string str)
	{
		return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
	}

	//是否存在该用户
	private bool CanRegister(string id)
	{
		//防sql注入
		if (!IsSafeStr(id))
			return false;
		//查询id是否存在
		string cmdStr = string.Format("select * from user where id='{0}';", id);
		MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
		try
		{
			MySqlDataReader dataReader = cmd.ExecuteReader();
			bool hasRows = dataReader.HasRows;
			dataReader.Close();
			return !hasRows;
		}
		catch (Exception e)
		{
			Console.WriteLine("[DataMgr]CanRegister fail " + e.Message);
			return false;
		}
	}


	//
	//	INSERT INTO Customers
	//VALUES(1000000006,
	//       'Toy Land',
	//       NULL,
	//	   NULL);
	public bool Insert(string str, int id)
	{
		//防sql注入
		if (!IsSafeStr(str))
		{
			Console.WriteLine("[DataMgr]Insert 使用非法字符");
			return false;
		}
		//能否注册
		if (!CanInsert(id))
		{

			Console.WriteLine("[DataMgr]Insert " + id + "存在");
			return false;
		}
		//写入数据库User表
		string cmdStr = string.Format("insert into info  (xuhao," +
														"jigui" +
														"pos" +
														"name" +
														"ip" +
														"status" +
														"pingpai" +
														"xinghao" +
														"year" +
														"cpu" +
														"mem" +
														"disk" +
														"bumen" +
														"contact" +
														"tel" +
														"mark" +
														"),values ({0}) ", str);
		MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);

		try
		{
			cmd.ExecuteNonQuery();
			return true;
		}
		catch (Exception e)
		{
			Debug.Log("[DataMgr]Insert " + e.Message);
			return false;
		}
	}
	public bool CanInsert(int id)
	{
		//防sql注入
		//if (!IsSafeStr(id))
		//	return false;
		//查询id是否存在
		string cmdStr = string.Format("select * from info where id={0};", id);
		MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
		try
		{
			MySqlDataReader dataReader = cmd.ExecuteReader();
			bool hasRows = dataReader.HasRows;
			dataReader.Close();
			return hasRows;
		}
		catch (Exception e)
		{
			Console.WriteLine("[DataMgr]CanRegister fail " + e.Message);
			return false;
		}
	}
	//注册
	public bool Modify(string phone, string pw)
	{
		//防sql注入
		if (!IsSafeStr(pw) || !IsSafeStr(phone))
		{
			Console.WriteLine("[DataMgr]Modify 使用非法字符");
			return false;
		}
		//能否注册
		if (CanRegister(phone))
		{
			Console.WriteLine("[DataMgr]Modify " + phone + "电话号码不存在");
			return false;
		}
		//写入数据库User表
		string cmdStr = string.Format("update user set pw ='{0}' where phone ='{1}';", pw, phone);
		MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
		try
		{
			cmd.ExecuteNonQuery();
			return true;
		}
		catch (Exception e)
		{
			Debug.Log("[DataMgr]Register " + e.Message);
			return false;
		}
	}
	public bool Update(int v)
	{

		//写入数据库User表
		string cmdStr = string.Format("select pw from user where phone='10000'");
		MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
		//byte[] buffer;
		try
		{
			MySqlDataReader dataReader = cmd.ExecuteReader();
			if (!dataReader.HasRows)
			{
				dataReader.Close();
				return false;
			}
			int i = -1;
			while (dataReader.Read())
			{
				i = int.Parse(dataReader[0].ToString());
			}
			dataReader.Close();
			if (i > v)
				return true;
		}
		catch (Exception e)
		{
			Console.WriteLine("[DataMgr]GetPlayerData 查询 " + e.Message);
			return false;
		}
		return false;
	}

	public DataTable GetInfo()
	{
		//SELECT COLUMN_NAME FROM USER_TAB_COLUMNS WHERE TABLE_NAME = '表名'

		//写入数据库User表
		string cmdStr = string.Format("select * from info");
		//if (sqlConn.State != ConnectionState.Open)
		//	sqlConn.Open();
		MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
		//DataTable dt = new DataTable();
		//dt.
		//byte[] buffer;
		if (isCON == false)
		{
			Connect();
		}
		DataTable dt = new DataTable();
		try
		{
			MySqlDataReader dataReader = cmd.ExecuteReader();
			if (!dataReader.HasRows)
			{
				dataReader.Close();
				Console.WriteLine("无数据");
				return null;
			}
			else
			{
				for (int i = 0; i < dataReader.FieldCount; i++)
				{
					//Console.Write(dataReader.GetName(i) + ",");
					dt.Columns.Add(dataReader.GetName(i));
				}
			}
			while (dataReader.Read())
			{
				DataRow dr = dt.NewRow();
				for (int i = 0; i < dataReader.FieldCount; i++)
				{
					dr[i] = dataReader[i];
				}
				dt.Rows.Add(dr);
				Console.WriteLine();
			}
			dataReader.Close();
			return dt;
		}
		catch (Exception e)
		{
			Debug.Log("[DataMgr]GetInfo 查询 " + e.Message);
			//dataReader.Close();
			sqlConn.Close();
			return null;
		}
	}


	//检测用户名密码
	public bool CheckPassWord(string id, string pw)
	{
		//防sql注入
		if (!IsSafeStr(id) || !IsSafeStr(pw))
			return false;
		//查询
		string cmdStr = string.Format("select * from user where id='{0}' and pw='{1}';", id, pw);
		MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
		try
		{
			MySqlDataReader dataReader = cmd.ExecuteReader();
			bool hasRows = dataReader.HasRows;
			dataReader.Close();
			return hasRows;
		}
		catch (Exception e)
		{
			Console.WriteLine("[DataMgr]CheckPassWord " + e.Message);
			return false;
		}
	}

	public PlayerData GetPlayerData(string id)
	{
		PlayerData playerData = null;
		//防sql注入
		if (!IsSafeStr(id))
			return playerData;
		playerData = new PlayerData();
		//查询
		// string cmdStr = string.Format("select * from user where phone ='{0}';", id);
		string cmdStr = string.Format("SELECT b.* from (SELECT t.*, @rownum:= @rownum + 1 AS rownum FROM(SELECT @rownum:= 0) r, user AS t " +
			" ORDER BY t.bestrecord ASC ) as b where b.phone = '{0}';", id);

		MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
		//byte[] buffer;
		try
		{
			MySqlDataReader dataReader = cmd.ExecuteReader();
			if (!dataReader.HasRows)
			{
				dataReader.Close();
				return playerData;
			}
			dataReader.Read();
			dataReader.Close();
			return playerData;
		}
		catch (Exception e)
		{
			Console.WriteLine("[DataMgr]GetPlayerData 查询 " + e.Message);
			return playerData;
		}

	}
}