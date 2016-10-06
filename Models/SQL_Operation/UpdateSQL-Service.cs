using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

namespace LongTermCare_Xml_.Models.SQL_Operation
{
    public class UpdateSQL_Service
    {
        public OleDbConnection DataConnection = null;
        public UpdateSQL_Service() { }
        ~UpdateSQL_Service()
        {
            System.Diagnostics.Trace.WriteLine("UpdateSQL_Service is clear");
        }

        public int Update(string OperationString, DataSet GlobalParam)
        {
            DataTable InputData = GlobalParam.Tables[0];
            string SQLCommand = "", TableName = "GlobalParam", Condition = " WHERE PKey = 0";
            OleDbCommand Command = null;
            OleDbTransaction transaction = null;
            SQLCommand = "UPDATE " + TableName + " SET ";
            int SQL_Column_Count = 0, Result = 0;
            //串聯新增字串(欄位名稱部分)
            foreach (DataColumn Data_Column in InputData.Columns)
            {
                if (Data_Column.ColumnName.Equals("PKey"))
                {
                    SQL_Column_Count++;
                    continue;
                }
                SQLCommand += Data_Column.ColumnName + " = ";
                if (InputData.Rows[0].ItemArray[SQL_Column_Count] is int)
                {
                    if (!OperationString.Equals("Series"))
                        SQLCommand += (Data_Column.ColumnName.Equals(OperationString + "Version")) ? int.Parse(InputData.Rows[0].ItemArray[SQL_Column_Count].ToString()) + 1 : int.Parse(InputData.Rows[0].ItemArray[SQL_Column_Count].ToString());
                    else
                    {
                        if (Data_Column.ColumnName.Equals("SeriesInstanceCount") || Data_Column.ColumnName.Equals("SeriesNumberCount"))
                            SQLCommand += int.Parse(InputData.Rows[0].ItemArray[SQL_Column_Count].ToString()) + 1;
                        else
                            SQLCommand += int.Parse(InputData.Rows[0].ItemArray[SQL_Column_Count].ToString());
                    }
                }
                else
                    SQLCommand += "'" + InputData.Rows[0].ItemArray[SQL_Column_Count].ToString() + "'";
                SQL_Column_Count++;
                SQLCommand += (SQL_Column_Count <= InputData.Columns.Count - 1) ? "," : "";
            }
            SQLCommand += Condition;
            //傳質入資料庫部分
            try
            {
                // Start a local transaction
                transaction = DataConnection.BeginTransaction();
                Command = new OleDbCommand(SQLCommand, DataConnection);
                Command.Transaction = transaction;
                Result = Command.ExecuteNonQuery();
                // Commit the transaction.
                transaction.Commit();
                Console.WriteLine("Both records are written to database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Attempt to roll back the transaction.
                transaction.Rollback();
            }
            return Result;
        }
    }
}