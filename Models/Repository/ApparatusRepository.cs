using LongTermCare_Xml_.Models.DTO_Models;
using LongTermCare_Xml_.Models.SQL_Operation;
using LongTermCare_Xml_.Models.Xml_Operation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;

namespace LongTermCare_Xml_.Models.Repository
{
    public class ApparatusRepository:IDisposable
    {
        ~ApparatusRepository()
        {
            Dispose();
        }
        public bool CheckMachineMAC(XmlDocument Xml)
        {
            try
            {
                DBOper_DTO DBDTO = new DBOper_DTO();
                string DBOper = "SELECT MACAddress , ModalityType , DeviceID , ChineseName , SOPClassUID FROM DeviceRegistration where DeviceRegistration.MACAddress = ?";
                List<string> TableList = new List<string>() { "DeviceRegistration" };
                BaseDBOpertion<DBOper_DTO> Method = new BaseDBOpertion<DBOper_DTO>(DBDTO);
                Method.SettingDBDTO(ref DBDTO, TableList, "203.64.84.113,1433", "global");
                Method.MappingXmltoDBDTO(ref DBDTO, Xml);
                DataTable dt = (DataTable)Method.BaseOperation(ref DBDTO, DBOper);
                return dt.Rows.Count != 0 ? true : false;
            }
            catch (Exception er)
            {
                er.ToString();
            }
            throw new Exception();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}