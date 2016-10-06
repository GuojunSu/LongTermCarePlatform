using LongTermCare_Xml_.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace LongTermCare_Xml_.Models.Xml_Operation
{
    public class ConvertXmltoObject
    {
        public Tout MappingXmltoObject<Tout>(Tout Object, XmlDocument Xml)
        {
            IEnumerable<XElement> InputXmlDoc = XDocument.Parse(Xml.OuterXml).Root.Elements();
            XElement _TempElement = null;
            try
            {
                foreach (PropertyInfo Prop in Object.GetType().GetProperties())
                {
                    _TempElement = InputXmlDoc.FirstOrDefault(el => el.Name == Prop.Name);
                    if (_TempElement != null)
                    {
                        Prop.SetValue(Object,_TempElement.Value);
                        _TempElement = null;
                    }
                    else
                    {
                        _TempElement = InputXmlDoc.FirstOrDefault(el => el.HasAttributes && el.Attribute("FieldName").Value == Prop.Name);
                        if (_TempElement != null)
                        {
                            Prop.SetValue(Object, _TempElement.Value);
                            _TempElement = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return (Tout)Object;
        }
    }

}