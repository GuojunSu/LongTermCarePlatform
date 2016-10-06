using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Web;

namespace LongTermCare_Xml_.Models.SQL_Operation
{
    public class DeleteSQL_Service
    {
        public OleDbConnection DataConnection = null;
        public DeleteSQL_Service() { }
        ~DeleteSQL_Service()
        {
            System.Diagnostics.Trace.WriteLine("DeleteSQL_Service is clear");
        }
        public int DeleteGlobalParam(String PrimaryKey)
        {
            string SQLCommand = "", TableName = "GlobalParam";
            OleDbCommand Command = null;
            SQLCommand = "DELETE FROM " + TableName + " WHERE " + PrimaryKey + "= 0";
            Command = new OleDbCommand(SQLCommand, DataConnection);
            return Command.ExecuteNonQuery();
        }
    }
}