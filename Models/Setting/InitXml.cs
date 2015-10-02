using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace LongTermCare_Xml_.Models.Setting
{
    public class InitXml
    {
        public XDocument StudyXml { get; set; }
        public XDocument InsertXml { get; set; }
        public InitXml(SettingInfo info)
        {
            InsertXml = XDocument.Load(info.InsertPath + "InsertModel\\Insert.xml");
            StudyXml = XDocument.Load(info.SearchPath + "SearchModel\\STUDYXML-1.xml");
        }
    }
}