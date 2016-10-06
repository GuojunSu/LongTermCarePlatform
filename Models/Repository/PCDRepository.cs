using LongTermCare_Xml_.Models.DTO_Models;
using LongTermCare_Xml_.Models.SQL_Operation;
using System;
using System.Xml;
using System.Xml.Linq;

namespace LongTermCare_Xml_.Models.Repository
{
    public class PCDRepository
    {
        private XDocument PCDXml;
        private XDocument SearchXml;
        DBOper_DTO DBDTO;
        BaseDBOpertion<DBOper_DTO> DBOper;
        InstitutionRespository dbname;

        public PCDRepository()
        {
            DBDTO = new DBOper_DTO();
            DBOper = new BaseDBOpertion<DBOper_DTO>(DBDTO);
            dbname = new InstitutionRespository();
        }
        //查詢操作
        public int SearchOperation(XmlDocument Search, string account,string UID)
        {
            string connectionstring;
            search SearchFun;
            int FileCount = 0;
            //轉換成XDocument
            SearchXml = XDocument.Parse(Search.OuterXml);
            //把結果給學弟
            try
            {
                SearchFun = new search();
                string _dbname = dbname.GetInstitutionDBName(UID);
                DBOper.SettingConnectionString(ref DBDTO,"203.64.84.113,1433", _dbname);
                connectionstring = DBOper.GetConnectionString(ref DBDTO);
                FileCount = SearchFun.ProcessXML(SearchXml, account, connectionstring);
            }
            catch (Exception e)
            {
                throw e.GetBaseException();
            }
            return FileCount;
        }

        //新增操作
        public int InsertOperation(XmlDocument PCD)
        {
            string connectionstring;
            //把XmlDocument轉換成XDocument
            insert InsertFun;
            PCDXml = XDocument.Parse(PCD.OuterXml);
            try
            {
                //把結果給學弟
                InsertFun = new insert();
                DBOper.SettingConnectionString(ref DBDTO, "203.64.84.113,1433");
                connectionstring = DBOper.GetConnectionString(ref DBDTO);
                InsertFun.ProcessXML(PCDXml, connectionstring);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException());
            }
            return 200;
        }
    }
}