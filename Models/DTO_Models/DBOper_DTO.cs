using LongTermCare_Xml_.Models.Interface;
using System.Collections.Generic;
using System.ComponentModel;
using static LongTermCare_Xml_.Models.SQL_Operation.TableColumnInfo;

namespace LongTermCare_Xml_.Models.DTO_Models
{
    public sealed class DBOper_DTO :IMethodObject
    {
        private Dictionary<string, List<string>> Strategy;
        [DefaultValue(null)]
        public List<TableInfo> _SQLQuery { get; set; }
        private string DataSource = "203.64.84.113,1433;";
        [Description("Data Source=")]
        public string _Data_Source
        {
            get { return DataSource; }
            set { DataSource = value; }
        }

        private string InitialCatalog = "PCD;";
        [Description("Initial Catalog=")]
        public string _Initial_Catalog
        {
            get { return InitialCatalog; }
            set { InitialCatalog = value; }
        }

        private string UserID = "sa;";
        [Description("User ID=")]
        public string _User_ID
        {
            get { return UserID; }
            set { UserID = value; }
        }

        private string Password = "tcumi123;";
        [Description("Password=")]
        public string _Password
        {
            get { return Password; }
            set { Password = value; }
        }

        public DBOper_DTO()
        {
            Strategy = new Dictionary<string, List<string>>();
            Strategy.Add("NotSetting",
                    new List<string>(
                        new string[] { "_SQLQuery" }));
        }

        public Dictionary<string, List<string>> Get_Object_Strategy()
        {
            return Strategy;
        }
    }
}