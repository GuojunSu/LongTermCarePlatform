using LongTermCare_Xml_.Models.DTO_Models;
using LongTermCare_Xml_.Models.SQL_Operation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;


/// <summary>
/// ReadSQL_Server 的摘要描述
/// </summary>
public class SearchSQL_Service : BaseSQL
{
    public OleDbConnection DataConnection { get; set; }
    public List<string> Search_Table_Name_List { get; set; }

    public SearchSQL_Service() { }
    ~SearchSQL_Service()
    {
        System.Diagnostics.Trace.WriteLine("SearchSQL_Service is clear");
    }

    public DataSet GetGlobalParam()
    {
        DataSet DataSet = new DataSet();
        OleDbCommand AccessCommand = null;
        OleDbDataAdapter DataAdapter = null;
        string SQLCommand, TableName = "GlobalParam";
        SQLCommand = "SELECT * FROM [" + TableName + "]";
        AccessCommand = new OleDbCommand(SQLCommand, DataConnection);
        DataAdapter = new OleDbDataAdapter(AccessCommand);
        DataAdapter.FillSchema(DataSet, SchemaType.Source, TableName);
        DataAdapter.Fill(DataSet, TableName);
        return DataSet;
    }

    public string GetXmlVersion(string OperationString)
    {
        int Count = 0;
        string SQL_Result = null;
        foreach (DataColumn dc in GetGlobalParam().Tables[0].Columns)
        {
            if (dc.ColumnName.Equals(OperationString + "Version"))
            {
                SQL_Result = GetGlobalParam().Tables[0].Rows[0].ItemArray[Count].ToString();
                break;
            }
            Count++;
        }
        return SQL_Result;
    }

    public DataSet Search_operation()
    {
        DataSet DataSet = new DataSet();
        OleDbCommand AccessCommand = null;
        OleDbDataAdapter DataAdapter = null;
        string SQLCommand;
        try
        {
            //確定連線
            if (DataConnection.State.Equals(ConnectionState.Open))
            {
                //從資料庫，抓全部的資料
                foreach (var Table_Name in Search_Table_Name_List)
                {
                    SQLCommand = "SELECT * FROM [" + Table_Name + "]";
                    AccessCommand = new OleDbCommand(SQLCommand, DataConnection);
                    DataAdapter = new OleDbDataAdapter(AccessCommand);
                    //把PrimaryKey資訊加入
                    DataAdapter.FillSchema(DataSet, SchemaType.Source, Table_Name);
                    DataAdapter.Fill(DataSet, Table_Name);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: Failed to retrieve the required data from the DataBase.\n{0}", ex.Message);
        }
        return DataSet;
    }

    public void Search_Conditional<InEntity>(InEntity DB_oper, ref DataSet Search_Buff)
    {
        OleDbCommand AccessCommand = null;
        OleDbDataAdapter DataAdapter = null;
        string SQLCommand = "SELECT";
        try
        {
            //取得前端條件式
            GetSQLObjects(DB_oper);
            //確定連線
            if (DataConnection.State.Equals(ConnectionState.Open))
            {
                //從資料庫，抓全部的資料
                AccessCommand = new OleDbCommand(SQLCommand, DataConnection);
                DataAdapter = new OleDbDataAdapter(AccessCommand);

            }
            //把PrimaryKey資訊加入
            //DataAdapter.FillSchema(Search_Buff, SchemaType.Source, Table_Name);
            //DataAdapter.Fill(Search_Buff, Table_Name);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: Failed to retrieve the required data from the DataBase.\n{0}", ex.Message);
        }
    }
}