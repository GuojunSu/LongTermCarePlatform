using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.IO;
/// <summary>
/// CreateDB 的摘要描述
/// </summary>
public class CreateDB
{
	public CreateDB()
	{
		//
		// TODO: 在這裡新增建構函式邏輯
		//
	}
    public int NewDB(string DBname,string UID)
    {
        StreamReader sr = new StreamReader(@"C:\Users\tcu\Desktop\CDB.txt");
        String line = sr.ReadToEnd();
        SqlConnection sqlCon = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        sqlCon.Open();

        if (DBname != null)
        {
            SqlCommand sqlCmd1 = new SqlCommand("CREATE DATABASE " + DBname, sqlCon);
            sqlCmd1.ExecuteReader();
            sqlCmd1 = new SqlCommand(line.Replace("DBname", DBname).Replace("TargetUID", UID), sqlCon);
            sqlCmd1.ExecuteReader();
        }
        sr.Close();
        sqlCon.Dispose();
        sqlCon.Close();
    }
}