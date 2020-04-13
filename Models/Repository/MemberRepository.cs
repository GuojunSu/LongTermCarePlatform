using LongTermCare_Xml_.Models.DTO_Models;
using LongTermCare_Xml_.Models.Interface;
using LongTermCare_Xml_.Models.Protal_Operation;
using LongTermCare_Xml_.Models.Xml_Operation;
using LongTermCare_Xml_.WebReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace LongTermCare_Xml_.Models.Repository
{
    public class MemberRepository
    {
        ~MemberRepository()
        {
            System.Diagnostics.Trace.WriteLine("MemberRepository is clear");
        }

        public OUT CreateMember<OUT>(object _object, OUT _result)
        {
            int tempvalue = -1;
            //檢查人員的註冊
            try
            {
                ConvertXmltoObject ConvertMethod = new ConvertXmltoObject();
                Protal_DTO Response = new Protal_DTO();
                Response = ConvertMethod.MappingXmltoObject(Response, (XmlDocument)_object);
                Response.Get_Portal_Type("病患").Get_Tree_No();
                tempvalue = Response.IsRegistration(-1);
                _result.GetType().GetProperties().SetValue(tempvalue.Equals(0) ? (Response.AddMember("").Equals("Success") ? 0 : -1) : tempvalue, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return _result;
        }
    }
}