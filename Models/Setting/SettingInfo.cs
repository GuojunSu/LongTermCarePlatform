using GuoJunSu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace LongTermCare_Xml_.Models.Setting
{
    public class SettingInfo
    {
        public string InsertPath { get; set; }
        public string SearchPath { get; set; }
        public SettingInfo()
        {
            //讀取內嵌資源
            string SettingFilePath = HttpContext.Current.Server.MapPath("~/App_GlobalResources/SettingFile.ini");
            Stream sourse = new FileStream(SettingFilePath,FileMode.Open,FileAccess.Read);
            ProcessINI ReadINI1 = new ProcessINI(sourse);
            InsertPath = ReadINI1.GetKeyValueString("Environment_Parameter", "InsertPath");
   
            Stream sourse2 = new FileStream(SettingFilePath, FileMode.Open, FileAccess.Read);
            ProcessINI ReadINI2 = new ProcessINI(sourse2);
            SearchPath = ReadINI2.GetKeyValueString("Environment_Parameter", "SearchPath");
        }
    }
}