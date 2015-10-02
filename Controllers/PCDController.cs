using LongTermCare_Xml_.Models.ProcessXml;
using LongTermCare_Xml_.Models.Setting;
using MemoryCacher;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;

namespace LongTermCare_Xml_.Controllers
{
    [RoutePrefix("Api/PCD")]
    public class PCDController : ApiController
    {
        private ProcessXml XmlOperation { get; set; }
        private SettingInfo Info { get; set; }
        private InitXml Xml { get; set; }
        public PCDController()
        {
            string ClassName = "ProcessXml", SetString = "SetPath", InitXmlDoc = "InitXml";
            MemoryCacherApi Cache = new MemoryCacherApi();

            //init setting
            if ((Info = (SettingInfo)Cache.GetValue(SetString)) == null)
            {
                Info = new SettingInfo();
                DateTimeOffset TimeOffset = DateTimeOffset.Now.AddMonths(1);
                Cache.AddCache(SetString, Info, TimeOffset);
            }

            //init xml
            if ((Xml = (InitXml)Cache.GetValue(InitXmlDoc)) == null)
            {
                Xml = new InitXml(Info);
                DateTimeOffset TimeOffset = DateTimeOffset.Now.AddHours(5);
                Cache.AddCache(InitXmlDoc, Xml, TimeOffset);
            }

            //init processxml
            if ((XmlOperation = (ProcessXml)Cache.GetValue(ClassName)) == null)
            {
                XmlOperation = new ProcessXml(Info, Xml);
                DateTimeOffset TimeOffset = DateTimeOffset.Now.AddHours(5);
                Cache.AddCache(ClassName, XmlOperation, TimeOffset);
            }
        }

        [HttpPost, Route("Insert")]
        public IHttpActionResult PCDCreate(XmlDocument PCD)
        {
            try
            {
                //要做log紀錄
                if (PCD != null)
                    XmlOperation.InsertOperation(PCD);
                else
                    return BadRequest();
            }
            catch (Exception)
            {
                return Conflict();
            }
            return Ok();
        }

        [HttpPost, Route("Search/{account}")]
        public HttpResponseMessage PCDSearch(XmlDocument SearchXml, string account)
        {
            int result = 0;
            try
            {
                if (SearchXml != null)
                    result = XmlOperation.SearchOperation(SearchXml, account);
                else
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
            }
            //包裹回傳質 
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent(JsonConvert.SerializeObject(result));
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return resp;
        }
    }
}
