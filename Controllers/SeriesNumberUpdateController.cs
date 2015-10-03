using LongTermCare_Xml_.Models.ProcessXml;
using LongTermCare_Xml_.Models.Setting;
using MemoryCacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Xml.Linq;

namespace LongTermCare_Xml_.Controllers
{
    [RoutePrefix("Api/SeriesNumber")]
    public class SeriesNumberUpdateController : ApiController
    {
        private ProcessXml XmlOperation { get; set; }
        private SettingInfo Info { get; set; }
        MemoryCacherApi Cache = new MemoryCacherApi();
        private InitXml Xml { get; set; }
        public SeriesNumberUpdateController()
        {
            XmlOperation = new ProcessXml();
        }
        //下載SeriesNumber列表
        [HttpGet, Route("Download")]
        public HttpResponseMessage SeriesNumberDownload()
        {
            XDocument UpdateDoc;
            try
            {
                string SetString = "SetPath";
                //init setting
                //if ((Info = (SettingInfo)Cache.GetValue(SetString)) == null)
                //{
                Info = new SettingInfo();
                //    DateTimeOffset TimeOffset = DateTimeOffset.Now.AddMonths(1);
                //    Cache.AddCache(SetString, Info, TimeOffset);
                //}
                //UpdateDoc = XmlOperation.OpenXml("Update", @"D:\C sharp code\LongTermCare(Xml)\Update\", "PATIENTXML-1.xml");
                UpdateDoc = XmlOperation.OpenXml("Update", Info.UpdatePath, "SeriesNumberTable.xml");
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
            }
            //包裹回傳質 
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent(UpdateDoc.ToString());
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/Xml");
            return resp;
        }
    }
}
