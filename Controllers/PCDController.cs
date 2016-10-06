using LongTermCare_Xml_.Models.Repository;
using LongTermCare_Xml_.Models.Setting;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Xml;

namespace LongTermCare_Xml_.Controllers
{
    [RoutePrefix("Api/PCD")]
    public class PCDController : ApiController
    {
        private PCDRepository PCD_Operation;
        //private SettingInfo Info { get; set; }
        //private InitXml Xml { get; set; }
        public PCDController()
        {
            PCD_Operation = new PCDRepository();
        }

        //新增PCD 資料
        [HttpPost, Route("Insert")]
        public IHttpActionResult PCDCreate(XmlDocument PCD)
        {
            try
            {
                if (PCD != null)
                {
                    SettingInfo Path_Info = new SettingInfo();
                    string PathStr = Path_Info.BasicPath;
                    PathStr += "Insert於" + DateTime.Now.ToString("HH點mm分ss秒") + "TempFile.xml";
                    PCD.Save(PathStr);
                    PCD_Operation.InsertOperation(PCD);
                }
                else
                    return BadRequest();
            }
            catch (Exception)
            {
                return Conflict();
            }
            return Ok();
        }

        //現在提供哪個帳號查詢
        [HttpPost, Route("Search/account={account},uid={UID}")]
        public HttpResponseMessage PCDSearch(XmlDocument SearchXml, string account,string UID)
        {
            int result = 0;
            try
            {
                if (SearchXml != null)
                    result = PCD_Operation.SearchOperation(SearchXml, account, UID);
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
