using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Reflection;

/// <summary>
/// Connection_DataBase 的摘要描述
/// </summary>
public class Connection_DataBase
{
    private OleDbConnection DBConn = null;
    public Connection_DataBase()
    {
        try
        {
            // new connection connection open
            var ConnStr = "Provider=SQLOLEDB;" + System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            DBConn = new OleDbConnection(ConnStr);
            DBConn.Open();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: Failed to retrieve the required data from the DataBase.\n{0}", ex.Message);
        }
    }

    public Connection_DataBase(object Input)
    {
        try
        {
            // new connection connection open
            string Constring = "Provider=SQLOLEDB;";
            Dictionary<string, List<string>> TEMP = (Dictionary<string, List<string>>)Input.GetType().GetMethod("Get_Object_Strategy").Invoke(Input, null);
            List<string> Strategy = TEMP.First().Value;
            foreach (var attri in Input.GetType().GetProperties())
            {
                List<CustomAttributeData> pInfo = attri.GetCustomAttributesData().ToList();
                object AttritudeValue = pInfo[0].ConstructorArguments[0].Value;
                if (AttritudeValue != null)
                {
                    if (Strategy.Where(valueName => AttritudeValue.ToString().Equals(valueName)).Count() == 0)
                        Constring = Constring + AttritudeValue.ToString() + attri.GetValue(Input).ToString();
                }
            }
            DBConn = new OleDbConnection(Constring);
            DBConn.Open();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: Failed to retrieve the required data from the DataBase.\n{0}", ex.Message);
        }
    }

    ~Connection_DataBase()
    {
        if (DBConn != null && IsDBConnection())
            CloseConnection();
        System.Diagnostics.Trace.WriteLine("DataBase_Connection is clear");
    }

    public void CloseConnection()
    {
        DBConn=null;
    }

    public OleDbConnection GetDBConObject()
    {
        return DBConn;
    }

    public bool IsDBConnection()
    {
        return DBConn.State.Equals(ConnectionState.Open) ? true : false;
    }
}