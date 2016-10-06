using LongTermCare_Xml_.Models.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LongTermCare_Xml_.Models
{
    public class Operation_Map
    {
        private SettingInfo Path_Info;
        private Dictionary<string, Dictionary<string, List<string>>> SQLDictionary = new Dictionary<string, Dictionary<string, List<string>>>();
        private Dictionary<string, List<string>> SQL_Operation_Dictionary = new Dictionary<string, List<string>>();
        private Dictionary<string, string> XmlDictionary = new Dictionary<string, string>();
        private List<string> SQLlist = new List<string>();
        private List<string> SQLlist1 = new List<string>();
        private List<string> SQLlist2 = new List<string>();
        public Operation_Map()
        {
            Path_Info = new SettingInfo();
            SQLlist.Add("Patient");
            SQLlist.Add("Study");
            SQLlist.Add("Series");
            SQL_Operation_Dictionary.Add("PatientPCDListUpdate", SQLlist);
            SQLDictionary.Add("PatientPCDList", SQL_Operation_Dictionary);
            XmlDictionary.Add("PatientPCDList", "Update");

            SQLlist1.Add("DeviceRegistration");
            SQL_Operation_Dictionary.Add("DeviceRegistrationListUpdate", SQLlist1);
            SQLDictionary.Add("DeviceRegistrationList", SQL_Operation_Dictionary);
            XmlDictionary.Add("DeviceRegistrationList", "Update");

            SQLlist2.Add("OperatorRegistration");
            SQL_Operation_Dictionary.Add("OperatorRegistrationListUpdate", SQLlist2);
            SQLDictionary.Add("OperatorRegistrationList", SQL_Operation_Dictionary);
            XmlDictionary.Add("OperatorRegistrationList", "Update");
        }

        public List<string> GetSQLArray(string OperationString = null, string RepositoryOperation = null)
        {
            Dictionary<string, List<string>> tempDictionary = SQLDictionary.Where(x => OperationString.Contains(x.Key)).Select(x => x.Value ).FirstOrDefault();
            return tempDictionary.Where(y => RepositoryOperation.Contains(y.Key)).Select(y => y.Value).FirstOrDefault();
        }

        public string GetXmlInfo(string OperationString = null)
        {
            return XmlDictionary.Where(x => OperationString.Contains(x.Key)).Select(x => x.Value).FirstOrDefault();
        }

        public string GetXmlPath(string OperationString = null)
        {
           return Path_Info.BasicPath + GetXmlInfo(OperationString)+"\\";
        }
    }
}