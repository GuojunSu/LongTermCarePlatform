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

        public string CreateMember(object _object)
        {
            string Result = "";
            //檢查人員的註冊
            try
            {
                ConvertXmltoObject ConvertMethod = new ConvertXmltoObject();
                Protal_DTO Response = new Protal_DTO();
                Response = ConvertMethod.MappingXmltoObject(Response, (XmlDocument)_object);
                Response.Get_Portal_Type("病患").Get_Tree_No();
                if (!Response.IsRegistration(false))
                {
                    if ((Result = Response.AddMember("")).Equals("Success"))
                        Console.WriteLine("註冊完成");
                }
                else
                    return "IsRegistration";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Result;
        }
    }
}