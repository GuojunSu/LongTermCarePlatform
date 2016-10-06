using LongTermCare_Xml_.Models.DTO_Models;
using LongTermCare_Xml_.Models.SQL_Operation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml;

namespace LongTermCare_Xml_.Models.Repository
{
    public class InstitutionRespository
    {
        public string GetInstitutionDBName(XmlDocument Xml)
        {
            string DBOper = "SELECT InstitutionName FROM InstitutionMapping WHERE InstitutionMapping.InstitutionUID = ?";
            List<string> TableList = new List<string>() { "InstitutionMapping" };
            DBOper_DTO DBDTO = new DBOper_DTO();
            BaseDBOpertion<DBOper_DTO> Method = new BaseDBOpertion<DBOper_DTO>(DBDTO);
            try
            {
                Method.SettingDBDTO(ref DBDTO, TableList, "203.64.84.113,1433", "global");
                Method.MappingXmltoDBDTO(ref DBDTO, Xml);
                DataTable dt = (DataTable)Method.BaseOperation(ref DBDTO, DBOper);
                return dt.Rows.Count != 0 ? dt.Rows[0].ItemArray[0].ToString() : "Err";
            }
            catch (Exception er)
            {
                er.ToString();
            }
            throw new Exception();
        }
        public string GetInstitutionDBName(string UID)
        {
            string DBOper = "SELECT InstitutionName FROM InstitutionMapping WHERE InstitutionMapping.InstitutionUID = "+"'" +UID + "'" ;
            List<string> TableList = new List<string>() { "InstitutionMapping" };
            DBOper_DTO DBDTO = new DBOper_DTO();
            BaseDBOpertion<DBOper_DTO> Method = new BaseDBOpertion<DBOper_DTO>(DBDTO);
            try
            {
                Method.SettingDBDTO(ref DBDTO, TableList, "203.64.84.113,1433", "global");
                DataTable dt = (DataTable)Method.BaseOperation(ref DBDTO, DBOper);
                return dt.Rows.Count != 0 ? dt.Rows[0].ItemArray[0].ToString() : "Err";
            }
            catch (Exception er)
            {
                er.ToString();
            }
            throw new Exception();
        }
    }
}