using LongTermCare_Xml_.Models.DTO_Models;
using LongTermCare_Xml_.Models.SQL_Operation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using static LongTermCare_Xml_.Models.SQL_Operation.TableColumnInfo;

namespace LongTermCare_Xml_.Models.Repository
{
    public sealed class RegistartionInfoRepsitory
    {
        InstitutionRespository InstitutionRespository = null;
        BaseDBOpertion<DBOper_DTO> Method = null;
        DBOper_DTO DBDTO = null;
        string DBName = null;
        List<string> TableList = new List<string>() { "Patient", "Study", "Series" };
        public RegistartionInfoRepsitory(ref XmlDocument InputXml)
        {
            DBDTO = new DBOper_DTO();
            Method = new BaseDBOpertion<DBOper_DTO>(DBDTO);
            InstitutionRespository = new InstitutionRespository();
            DBName = InstitutionRespository.GetInstitutionDBName(InputXml);
        }
        public int RegistrationInfo(ref XmlDocument InputXml)
        {
            int count = 0;
            string UpdateOpr = "UPDATE GlobalParam SET GlobalParam.SeriesInstanceCount = ? , GlobalParam.SeriesNumberCount = ? , GlobalParam.StudyInstanceCount = ? WHERE PKey = 0";
            List<string> SettingInfos = new List<string>() { "StudyInstanceUID", "StudyDate", "StudyTime", "StudyID", "SeriesInstanceUID", "SeriesNumber", "Modality", "SeriesDate", "SeriesTime" };
            string[] InsertDBOper = { "INSERT INTO Patient values ( ? , ? , ? , ? , ? ) ",
                "INSERT INTO Study values ( ? , ? , ? , ? , ? , ? , ? , ? , ? ) ",
                "INSERT INTO Series values ( ? , ? , ? , ? , ? , ? ) "
            };
            DataTable[] temp = new DataTable[InsertDBOper.Count()];
            try
            {
                string UID = GetXmlValue(ref InputXml, "InstitutionUID");
                //取得全域變數
                DataTable GobalTable = GetGlobalParam(DBName, ref InputXml);
                XDocument xdoc = XDocument.Parse(InputXml.OuterXml);
                foreach (string SetInfo in SettingInfos)
                    SetXmlValue(ref xdoc, SetInfo, GenerateValue(UID, ref GobalTable, SetInfo));
                InputXml.Load(xdoc.CreateReader());
                UpdateGobalValue(ref DBDTO, ref GobalTable);
                //更新全域變數
                Method.BaseOperation(ref DBDTO, UpdateOpr);
                Method.SettingDBDTO(ref DBDTO, TableList, "203.64.84.113,1433", DBName);
                //XDocument Convert to XmlDocument
                Method.MappingXmltoDBDTO(ref DBDTO, InputXml);
                foreach (string Oper in InsertDBOper)
                    temp[count++] = (DataTable)Method.BaseOperation(ref DBDTO, Oper);

            }
            catch (Exception er)
            {
                er.ToString();
            }
            finally
            {
                for (int i = 0; i < temp.Count(); i++)
                {
                    InsertDBOper[i] = null;
                    if (temp[i] != null)
                        temp[i].Clear();
                    temp[i] = null;
                }
                temp = null;
                SettingInfos.Clear();
            }
            return 101;
        }

        public int GetRegistrationInfo(ref XmlDocument InputXml)
        {
            string DBOper = "Select * from Patient left join Study on Patient.PatientID = Study.PatientID left join Series on Study.StudyInstanceUID = Series.StudyInstanceUID where Patient.PatientID = ?";
            //取得資料庫名稱
            try
            {
                Method.SettingDBDTO(ref DBDTO, TableList, "203.64.84.113,1433", DBName);
                Method.MappingXmltoDBDTO(ref DBDTO, InputXml);
                DataTable dt = (DataTable)Method.BaseOperation(ref DBDTO, DBOper);
                MappingDTtoXml(ref InputXml,ref dt);
            }
            catch (Exception er)
            {
                er.ToString();
            }
            return 101;
        }

        private DataTable GetGlobalParam(string DBName, ref XmlDocument InputXml)
        {
            string SearchDBOper = "SELECT StudyInstanceCount , SeriesInstanceCount , SeriesNumberCount FROM GlobalParam Where GlobalParam.UID = ?";
            List<string> GobalTable = new List<string>() { "GlobalParam" };
            Method.SettingDBDTO(ref DBDTO, GobalTable, "203.64.84.113,1433", DBName);
            ColumnAttrbute temp = DBDTO._SQLQuery[0]._Table.Where(ob => ob._ColumnName == "UID").First();
            temp._ColumnValue = GetXmlValue(ref InputXml, "InstitutionUID");
            return (DataTable)Method.BaseOperation(ref DBDTO, SearchDBOper);
        }

        private void SetXmlValue(ref XDocument xdoc, string Condition, object Value)
        {
            try
            {
                IEnumerable<XElement> Elements = xdoc.Root.Elements();
                XElement ele = null;
                ele = Elements.FirstOrDefault(el => el.Name == Condition);
                if (ele == null)
                    ele = Elements.FirstOrDefault(el => el.HasAttributes && el.Attribute("FieldName").Value == Condition);
                if (Value != null)
                    ele.SetValue(Value);
            }
            catch (Exception er)
            {
                er.ToString();
            }
        }

        private string GetXmlValue(ref XmlDocument InputXml, string Condition)
        {
            return XDocument.Parse(InputXml.OuterXml).Root.Elements().FirstOrDefault(el => el.Name == Condition).Value!=null? XDocument.Parse(InputXml.OuterXml).Root.Elements().FirstOrDefault(el => el.Name == Condition).Value : XDocument.Parse(InputXml.OuterXml).Root.Elements().FirstOrDefault(el => el.HasAttributes && el.Attribute("FieldName").Value == Condition).Value;
        }

        private object GenerateValue(string UID, ref DataTable GobalTable, string Condition)
        {
            List<string> MethodSetting = new List<string>() { "UID", "Date", "Time" };
            List<string> TableNameSetting = new List<string>() { "Study", "Series", "SeriesNumber" };
            object Result = null;
            try
            {
                switch (GetConditionIndex(MethodSetting, Condition))
                {
                    case 0:
                        Result = UID + "." + GobalTable.Rows[0].ItemArray[GetConditionIndex(TableNameSetting, Condition)];
                        break;
                    case 1:
                        Result = DateTime.Now.ToString("yyyyMMdd");
                        break;
                    case 2:
                        Result = DateTime.Now.ToString("HHmmss");
                        break;
                    default:
                        int index = GetConditionIndex(TableNameSetting, Condition);
                        Result = (index == 0) ? GobalTable.Rows[0].ItemArray[index] : DateTime.Now.ToString("yyyyMMdd.HHmmss.") + GobalTable.Rows[0].ItemArray[index];
                        break;
                }
            }
            catch (Exception er)
            {
                er.ToString();
            }
            return Result;
        }
        private int GetConditionIndex(List<string> SettingInfos, string Condition)
        {
            int count = 0;
            foreach (string Info in SettingInfos)
            {
                if (Condition.IndexOf(Info, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return count;
                }
                else
                    count++;
            }
            return -1;
        }
        private void UpdateGobalValue(ref DBOper_DTO _DBDTO, ref DataTable GobalTable)
        {
            BaseSQL DBDTO_Oper = new BaseSQL();
            ColumnAttrbute tempcol = null;
            int rowcount = 0;
            List<TableInfo> TempList = (List<TableInfo>)DBDTO_Oper.GetSQLObject(_DBDTO).GetValue(_DBDTO);
            foreach (DataColumn _ColName in GobalTable.Columns)
            {
                if ((tempcol = TempList.First()._Table.Where(CA => CA._ColumnName == _ColName.ColumnName).FirstOrDefault()) != null)
                {
                    int value = Convert.ToInt32(GobalTable.Rows[0].ItemArray[rowcount++]);
                    value++;
                    tempcol._ColumnValue = value;
                }
            }
        }
        private void MappingDTtoXml(ref XmlDocument InputXml, ref DataTable GobalTable)
        {
            int rowcount = 0;
            XDocument xdoc = XDocument.Parse(InputXml.OuterXml);
            foreach (DataColumn _ColName in GobalTable.Columns)
            {
                SetXmlValue(ref xdoc, _ColName.ColumnName, GobalTable.Rows[0].ItemArray[rowcount++]);
            }
            InputXml.Load(xdoc.CreateReader());
        }
    }
}