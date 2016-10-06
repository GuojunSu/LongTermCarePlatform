using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

namespace LongTermCare_Xml_.Models.SQL_Operation
{
    public class InsertSQL_Service
    {
        public OleDbConnection DataConnection = null;

        public InsertSQL_Service() { }
        ~InsertSQL_Service()
        {
            System.Diagnostics.Trace.WriteLine("InsertSQL_Service is clear");
        }

        public int InsertXmlVersion(string OperationString, DataTable InputData)
        {
            OleDbCommand AccessCommand = null;
            int SQL_Column_Count = 0;
            string SQLCommand, TableName = "GlobalParam";
            //先依照 table name 查出欄位名稱，再把 json 要新增的資料串接上查詢字串
            SQLCommand = "INSERT INTO [" + TableName + "]  ( ";

            //串聯新增字串(欄位名稱部分)
            foreach (DataColumn Data_Column in InputData.Columns)
            {
                if (SQL_Column_Count < InputData.Columns.Count - 1)
                    SQLCommand += Data_Column.ColumnName + " , ";
                else
                    SQLCommand += Data_Column.ColumnName;

                SQL_Column_Count++;
            }
            SQLCommand += " ) VALUES  (";

            //串聯數質得部分
            for (int RowCount = 0; RowCount < InputData.Rows[0].ItemArray.Count(); RowCount++)
            {
                if (RowCount < InputData.Columns.Count - 1)
                {
                    if (InputData.Columns[RowCount].ColumnName.Equals("UID"))
                        SQLCommand = SQLCommand + " '" + InputData.Rows[0].ItemArray[RowCount].ToString() + "' , ";
                    else
                        SQLCommand += InputData.Rows[0].ItemArray[RowCount].ToString() + " , ";
                }
                else
                {
                    int Temp = int.Parse(InputData.Rows[0].ItemArray[RowCount].ToString()) + 1;
                    SQLCommand += Temp.ToString();
                }
            }
            SQLCommand += " );";
            AccessCommand = new OleDbCommand(SQLCommand, DataConnection);
            return AccessCommand.ExecuteNonQuery();
        }
    }
}