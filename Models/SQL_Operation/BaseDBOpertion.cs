using LongTermCare_Xml_.Models.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using static LongTermCare_Xml_.Models.SQL_Operation.TableColumnInfo;

namespace LongTermCare_Xml_.Models.SQL_Operation
{
    public class BaseDBOpertion<InEntity> : BaseSQL
    {
        private Connection_DataBase SQLCon { get; set; }
        private SearchSQL_Service SQL_Seaarch { get; set; }
        private IMethodObject Method { get; set; }

        public BaseDBOpertion(IMethodObject Method)
        {
            this.Method = Method;
        }

        public virtual void SettingConnectionString(ref InEntity InputObject, params string[] Config)
        {
            int ParamTotalCount = 0, ParamCount = 0;
            ParamTotalCount = ParamCount = Config.Count();
            try
            {
                List<string> Strategy = Method.Get_Object_Strategy().First().Value;
                foreach (PropertyInfo attri in InputObject.GetType().GetProperties())
                {
                    //assume 
                    if (Strategy.Any(valueName => attri.Name.Equals(valueName)) && ParamCount != 0);
                    else
                    {
                        InitDBDTOConString(ref InputObject, attri, Config[ParamTotalCount - ParamCount] + ";");
                        ParamCount--; ;
                        ParamCount--;
                    }
                }
            }
            catch (Exception err)
            {
                err.ToString();
            }
        }

        public virtual void SettingDBDTO(ref InEntity InputObject, List<string> TableList, params string[] Config)
        {
            int ParamTotalCount = 0, ParamCount = 0;
            PropertyInfo TempAttri = null;
            ParamTotalCount = ParamCount = Config.Count();
            try
            {
                List<string> Strategy = Method.Get_Object_Strategy().First().Value;
                foreach (PropertyInfo attri in InputObject.GetType().GetProperties())
                {
                    //檢查是否為table的欄位
                    if (Strategy.Any(valueName => attri.Name.Equals(valueName)))
                        TempAttri = attri;
                    else if (Config.Count() != 0 && ParamCount != 0)
                    {
                        InitDBDTOConString(ref InputObject, attri, Config[ParamTotalCount - ParamCount] + ";");
                        ParamCount--;
                    }
                }
                //先初始化連接字串，在初始化TableInfo
                InitTableInfo(ref InputObject, TempAttri, TableList);
            }
            catch (Exception err)
            {
                err.ToString();
            }
        }
        private void InitTableInfo(ref InEntity InputObject, PropertyInfo Attri, List<string> TableList)
        {
            try
            {
                List<TableInfo> _TableList = (List<TableInfo>)Attri.GetValue(InputObject);
                _TableList = new List<TableInfo>();
                foreach (string TableName in TableList)
                {
                    string SQLOper = "SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME =";
                    SQLOper += "'" + TableName + "'";
                    //處理查詢語句
                    Object TempObj = BaseOperation(ref InputObject, SQLOper);
                    DataTable ResultDT = TempObj as DataTable;
                    TableInfo _TempTable = null;
                    List<ColumnAttrbute> Table = null;
                    if (ResultDT != null)
                    {
                        _TempTable = new TableInfo();
                        Table = new List<ColumnAttrbute>();
                        //把查到的資料放入物件
                        MappingDbTabletoColumnAttrbute(ref Table, ResultDT);
                        _TempTable._Table = Table;
                        _TempTable._TableName = TableName;
                        _TempTable._TableCount = Table.Count;
                        _TableList.Add(_TempTable);
                    }
                }
                Attri.SetValue(InputObject, _TableList, null);
            }
            catch (Exception er)
            {
                er.ToString();
            }
        }
        private void MappingDbTabletoColumnAttrbute(ref List<ColumnAttrbute> Table, DataTable ResultDT)
        {
            try
            {
                ColumnAttrbute temp = null;
                foreach (DataRow row in ResultDT.Rows)
                {
                    temp = new ColumnAttrbute();
                    PropertyInfo[] temps = temp.GetType().GetProperties();
                    temps[0].SetValue(temp, row.ItemArray[0], null);
                    temps[2].SetValue(temp, row.ItemArray[1], null);
                    Table.Add(temp);
                }
            }
            catch (Exception er)
            {
                er.ToString();
            }
        }
        private void InitDBDTOConString(ref InEntity InputObject, PropertyInfo Attri, string String)
        {
            Attri.SetValue(InputObject, String);
        }

        public virtual void MappingXmltoDBDTO(ref InEntity InputObject, XmlDocument MappingXml)
        {
            IEnumerable<XElement> InputXmlDoc = XDocument.Parse(MappingXml.OuterXml).Root.Elements();
            XElement _TempElement = null;
            List<TableInfo> list = (List<TableInfo>)GetSQLObject(InputObject).GetValue(InputObject);
            foreach (TableInfo _table in list)
            {
                try
                {
                    foreach (ColumnAttrbute _column in _table._Table)
                    {
                        _TempElement = InputXmlDoc.FirstOrDefault(el => el.Name == _column._ColumnName) != null ? InputXmlDoc.FirstOrDefault(el => el.Name == _column._ColumnName) : InputXmlDoc.FirstOrDefault(el => el.HasAttributes && el.Attribute("FieldName").Value == _column._ColumnName);
                        _column._ColumnValue = _TempElement != null ? _TempElement.Value : null;
                        _TempElement = null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 連結資料庫
        /// </summary>
        /// <param name="InputObject"></param>
        private void SQLConnection(ref InEntity InputObject)
        {
            SQLCon = new Connection_DataBase(InputObject);
        }
        public string GetConnectionString(ref InEntity Input)
        {
            try
            {
                // new connection connection open
                string Constring = "";
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
                return Constring;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: Failed to retrieve the required data from the DataBase.\n{0}", ex.Message);
            }
            return null;
        }
        /// <summary>
        /// 查詢資料庫資料放入DataSet中
        /// </summary>
        /// <param name="InputObject">資料庫鏈結字串設定;資料庫查詢語法設定</param>
        /// <param name="DataSet">從資料庫存放的資料集合</param>
        public virtual object BaseOperation(ref InEntity InputObject, string SQLModel)
        {
            try
            {
                OleDbCommand command = new OleDbCommand(SQLModel);
                //取得參數名稱
                Dictionary<string, List<string>> ColumnName = null;
                //假如有參數，把參數放進去SQLModel中
                if (GetAttriCount(SQLModel) > 0)
                {
                    ColumnName = GetAttributeName(SQLModel);
                    //取得SQL物件
                    List<TableInfo> tableList = (List<TableInfo>)GetSQLObject(InputObject).GetValue(InputObject);
                    foreach (TableInfo table in tableList)
                    {
                        if (ColumnName.Any(el => el.Key.Equals(table._TableName)))
                        {
                            List<string> AttriNameList = ColumnName.Where(el => el.Key.Equals(table._TableName)).Select(el => el.Value).FirstOrDefault();
                            if (AttriNameList[0].ToLower().Equals("insert"))
                            {
                                foreach (ColumnAttrbute ColAttri in table._Table)
                                    command.Parameters.Add(new OleDbParameter("@" + table + "." + ColAttri._ColumnName, GetAttributeValue(table._Table, ColAttri._ColumnName)._ColumnValue));
                            }
                            else
                            {
                                foreach (string AttriName in AttriNameList)
                                    command.Parameters.Add(new OleDbParameter("@" + table + "." + AttriName, GetAttributeValue(table._Table, AttriName)._ColumnValue));
                            }
                        }
                    }
                }

                if (SQLCon == null || !SQLCon.IsDBConnection())
                    SQLConnection(ref InputObject);

                command.Connection = SQLCon.GetDBConObject();
                //執行指令讀取結果
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    return dt;
                }
            }
            catch (Exception err)
            {
                err.ToString();
            }
            return null;
        }
        private ColumnAttrbute GetAttributeValue(List<ColumnAttrbute> AttrSet, string Condition)
        {
            return AttrSet.Where(attri => attri._ColumnName.Equals(Condition)).First();
        }
        /// <summary>
        /// 取得參數名稱
        /// </summary>
        /// <param name="SQLModel"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> GetAttributeName(string SQLModel)
        {
            Dictionary<string, List<string>> AttributeNameList = new Dictionary<string, List<string>>();
            string MatchPattern = "[\x3F]";
            int AttriCount = GetAttriCount(SQLModel, MatchPattern), BeginCount = 0;
            char[] delimiterChars = { ' ' }, _delimiterChars = { '.' };
            try
            {
                string[] _SQLModel = SQLModel.Split(delimiterChars);
                if (_SQLModel[0].ToUpper().Equals("INSERT"))
                {
                    List<string> temp = new List<string>();
                    temp.Add("INSERT");
                    AttributeNameList.Add(_SQLModel[2], temp);
                    return AttributeNameList;
                }
                do
                {
                    BeginCount = FindIndex(ref BeginCount, _SQLModel, "?");
                    string[] Attri = _SQLModel[BeginCount - 2].Split(_delimiterChars);
                    if (!AttributeNameList.Any(key => key.Key.Equals(Attri[0])))
                    {
                        List<string> temp = new List<string>();
                        temp.Add(Attri[1]);
                        AttributeNameList.Add(Attri[0], temp);
                    }
                    else
                        AttributeNameList[Attri[0]].Add(Attri[1]);

                    AttriCount--;
                } while (AttriCount != 0);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return AttributeNameList;
        }
        private int FindIndex<T>(ref int BeginIndex, IEnumerable<T> items, String MatchPattern)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (BeginIndex != 0)
                BeginIndex += 1;
            try
            {
                for (; BeginIndex < items.Count(); BeginIndex++)
                {
                    if (items.ElementAt(BeginIndex).Equals(MatchPattern))
                        return BeginIndex;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return -1;
        }
        /// <summary>
        /// 檢視SQLModel有幾個Parameter參數個數
        /// </summary>
        /// <param name="SQLModel"></param>
        /// <returns></returns>
        private int GetAttriCount(string SQLModel, string Pattern = null)
        {
            int Length = -1;
            string _Pattern = "[\x3F]";
            if (Pattern != null)
                _Pattern = Pattern;
            try
            {
                Length = Regex.Matches(SQLModel, _Pattern).Count;
            }
            catch (Exception er)
            {
                er.ToString();
            }
            return Length;
        }
    }
}