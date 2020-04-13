using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Services.Description;
using LongTermCare_Xml_.WebReference;
using LongTermCare_Xml_.Models;
using System.Xml;
using LongTermCare_Xml_.Models.Repository;
using LongTermCare_Xml_.Models.Interface;
using System.Xml.Linq;
using LongTermCare_Xml_.Models.Xml_Operation;
using LongTermCare_Xml_.Filters;

namespace LongTermCare_Xml_.Controllers
{
    [RoutePrefix("SecurityApi/MemberRegistration")]
    public class MemberRegistrationController : ApiController
    {
        public delegate int MethodDelegate(ref XmlDocument y);
        [HttpPost, Route("Registration")]
        // POST api/<controller>
        public IHttpActionResult RegistrationMember(XmlDocument Comsumer)
        {
            XDocument xdoc = null;
            int result = 0;
            ApparatusRepository ApparatusRepository = new ApparatusRepository();
            MemberRepository MemberRepository = new MemberRepository();
            RegistartionInfoRepsitory RegistartionInfoRepsitory = new RegistartionInfoRepsitory(ref Comsumer);
            SettingXmlValue SetXml = new SettingXmlValue();
            try
            {

                //檢查機器是否註冊
                if (!ApparatusRepository.CheckMachineMAC(Comsumer))
                {
                    SetXml.SetXmlValue(ref xdoc, "Registration_Status", 2);
                    Comsumer.Load(xdoc.CreateReader());
                    return Content(HttpStatusCode.OK, Comsumer);
                }
                MethodDelegate Method = null;
                //註冊人員
                switch (MemberRepository.CreateMember(Comsumer, -1))
                {
                    case 0:
                        Method += RegistartionInfoRepsitory.RegistrationInfo;
                        result = 1;
                        break;
                    case 1:
                        result = 6;
                        break;
                    case 2:
                        Method += RegistartionInfoRepsitory.GetRegistrationInfo;
                        result = 5;
                        break;
                    default:
                        throw new Exception();
                }
                if (Method(ref Comsumer).Equals(101))
                {
                    xdoc = XDocument.Parse(Comsumer.OuterXml);
                    SetXml.SetXmlValue(ref xdoc, "Registration_Status", result);
                    Comsumer.Load(xdoc.CreateReader());
                    return Ok(Comsumer);
                }
            }catch (Exception er)
            {
                return Content(HttpStatusCode.ExpectationFailed, er.ToString());
            }
            return Content(HttpStatusCode.GatewayTimeout, "failed");
        }
    }

}
