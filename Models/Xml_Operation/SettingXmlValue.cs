using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace LongTermCare_Xml_.Models.Xml_Operation
{
    public class SettingXmlValue
    {
        public void SetXmlValue(ref XDocument xdoc, string Condition, object Value)
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
    }
}