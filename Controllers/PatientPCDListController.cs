using LongTermCare_Xml_.Models.Interface;
using LongTermCare_Xml_.Models.Process_Xml;
using LongTermCare_Xml_.Models.Repository;
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
    [RoutePrefix("Api/PatientPCDList")]
    public class PatientPCDListController : ApiController
    {
        private Repository<string> Repositorys;
        private HttpResponseMessage resp;
     
        public PatientPCDListController()
        {
            Repositorys = new Repository<string>();
            Repositorys.OperationString = "PatientPCDList";
        }
        //下載SeriesNumber列表
        [HttpGet, Route("Download")]
        public HttpResponseMessage Download()
        {
            try
            {
                resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(Repositorys.Download_List());
                resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/Xml");
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
            }
            return resp;
        }

        //檢查資料庫是否有更新
        [HttpGet, Route("GetVersion")]
        public HttpResponseMessage GetVersion()
        {
            try
            {
                resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(Repositorys.Get_ListVersion());
                resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/Xml");
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
            }
            return resp;
        }

        //資料庫更新
        [HttpGet, Route("Update")]
        public HttpResponseMessage Update()
        {
            try
            {
                resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(Repositorys.Update_List().ToString());
                resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/Xml");
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.ExpectationFailed);
            }
            return resp;
        }
    }
}
