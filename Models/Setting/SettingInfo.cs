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
        public string BasicPath { get; set; }
        public SettingInfo()
        {
            //讀取內嵌資源
            string SettingFilePath = HttpContext.Current.Server.MapPath("~/App_GlobalResources/SettingFile.ini");
            Stream sourse = new FileStream(SettingFilePath, FileMode.Open, FileAccess.Read);
            ProcessINI ReadINI = new ProcessINI(sourse);
            BasicPath = ReadINI.GetKeyValueString("Environment_Parameter", "BasicPath");
            sourse.Close();
        }
    }
}