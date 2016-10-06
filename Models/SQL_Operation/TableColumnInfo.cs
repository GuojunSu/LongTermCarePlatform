using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LongTermCare_Xml_.Models.SQL_Operation
{
    public sealed class TableColumnInfo
    {
        public class ColumnAttrbute
        {
            public string _ColumnName { get; set; }
            public object _ColumnValue { get; set; }
            public string _ColumnType { get; set; }
        }
        public class TableInfo
        {
            public string _TableName { get; set; }
            public int _TableCount { get; set; }
            public string _PrimaryKey { get; set; }
            public List<ColumnAttrbute> _Table { get; set; }
        }
    }
}