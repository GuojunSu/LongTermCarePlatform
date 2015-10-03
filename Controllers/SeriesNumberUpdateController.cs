using LongTermCare_Xml_.Models.ProcessXml;
using LongTermCare_Xml_.Models.Setting;
using MemoryCacher;
using System;
using System.Collections.Generic;
using System.IO;
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
        SettingInfo PathInfo { get; set; }
        MemoryCacherApi Cache = new MemoryCacherApi();
        private InitXml Xml { get; set; }
        public SeriesNumberUpdateController()
        {
            string SetString = "Path2";
            //init setting
            if ((PathInfo = (SettingInfo)Cache.GetValue(SetString)) == null)
            {
                PathInfo = new SettingInfo();
               PathInfo.UpdatePath = @"D:\C sharp code\LongTermCare(Xml)\Update\";
                DateTimeOffset TimeOffset = DateTimeOffset.Now.AddMonths(1);
                Cache.AddCache(SetString, PathInfo, TimeOffset);
            }
            XmlOperation = new ProcessXml(PathInfo);
        }
        //下載SeriesNumber列表
        [HttpGet, Route("Download")]
        public HttpResponseMessage SeriesNumberDownload()
        {
            XDocument UpdateDoc = null;
            try
            {
                UpdateDoc = XmlOperation.OpenXml("Update", PathInfo.UpdatePath, "SeriesNumberTable.xml");
                if (UpdateDoc == null)
                    return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
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

        //檢查SeriesNumber列表是否存在?
        [HttpGet, Route("isExists")]
        public HttpResponseMessage SeriesNumberIsExists()
        {
            try
            {
                // 判斷 SeriesNumber Xml 是否存在  
                string FileName = PathInfo.UpdatePath + "SeriesNumberTable.xml";
                if (File.Exists(FileName))
                    return new HttpResponseMessage(HttpStatusCode.Found);
                else
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
            }
        }

    }
}
