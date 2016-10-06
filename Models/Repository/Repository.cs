using LongTermCare_Xml_.Models.DTO_Models;
using LongTermCare_Xml_.Models.Interface;
using LongTermCare_Xml_.Models.Process_Xml;
using LongTermCare_Xml_.Models.Setting;
using LongTermCare_Xml_.Models.SQL_Operation;
using System.Data;
using System.Xml.Linq;

namespace LongTermCare_Xml_.Models.Repository
{
    public class Repository<TEntity> : IRepository<TEntity>
    {
        private Connection_DataBase DB_Con;
        private SearchSQL_Service DB_Search;
        private UpdateSQL_Service DB_Update;
        private ProcessXml Xml_Operation;
        private DBOper_DTO DB_Oper = null;
        private BaseDBOpertion<DBOper_DTO> DB_Method;
        private Operation_Map OperationMapTable;
 
        
        //用來判別controller
        public string OperationString { get; set; }
        //用來判別操作
        private string RepositoryOperation { get; set; }

        public Repository()
        {
            OperationMapTable = new Operation_Map();
            DB_Oper = new DBOper_DTO();
            DB_Method = new BaseDBOpertion<DBOper_DTO>(DB_Oper);
            DB_Method.SettingConnectionString(ref DB_Oper);
        }
        ~Repository()
        {
            System.Diagnostics.Trace.WriteLine("Repository is clear");
        }

        /*
         * 下載更新請呼叫此功能
         * Step 0 : 讀入準備好的列表
         */
        public string Download_List()
        {
            //調閱Xml更新列表回來
            Xml_Operation = new ProcessXml();
            Xml_Operation.InitXmlPath(OperationMapTable.GetXmlPath(OperationString), "Update_Xml\\", OperationString);
            return Xml_Operation.OpenXml().ToString();
        }

        /*
         * 檢查列表版本請呼叫此功能
         * Step 1 : 開啟資料庫，查出結果
         */
        public string Get_ListVersion()
        {
            string SQL_Result = null;
            RepositoryOperation = "ListVersion";
            DB_Con = new Connection_DataBase(DB_Oper);
            if (DB_Con.IsDBConnection())//判斷資料庫連結
            {
                DB_Search = new SearchSQL_Service();
                //設定資料庫連結
                DB_Search.DataConnection = DB_Con.GetDBConObject();
                SQL_Result = DB_Search.GetXmlVersion(OperationString);
                DB_Con.CloseConnection();
                return SQL_Result;
            }
            else
                return "<:~資料庫連結，出了問題~:>";
        }

        /*
         * 假如有更新請呼叫此功能
         * Step 1 : 讀入模板
         * Step 2 : 開啟資料庫，查出結果存入DateSet中
         * Step 3 : 利用DateSet中資料，產生新的更新列表
         */
        public string Update_List()
        {
            RepositoryOperation = "Update";
            XDocument Xml_file_Stream = null;
            DataSet Search_Result = null;
            string Result;
            //Step1 讀入模板
            Xml_file_Stream = Xml_Part();
            if (Xml_file_Stream == null)
                return "<:~讀取Xml檔，出了問題，請檢查設定~:>";
            //Step2 結果存入DateSet
            DB_Con = new Connection_DataBase(DB_Oper);
            if (DB_Con.IsDBConnection())//判斷資料庫連結
            {
                Search_Result = SQL_SearchPart();
                DB_Con.CloseConnection();
            }
            else
                return "<:~資料庫連結，出了問題~:>";
            //Step3 產生新的更新列表   
            Xml_Operation.XmlDoc = Xml_file_Stream;
            Xml_Operation.DB_DataSet = Search_Result;
            Result = Xml_Operation.CreateXml();

            //Step4 更新版本
            DB_Con = new Connection_DataBase(DB_Oper);
            if (DB_Con.IsDBConnection())//判斷資料庫連結
            {
                DataSet TempData = null;
                //Search
                DB_Search = new SearchSQL_Service();
                DB_Search.DataConnection = DB_Con.GetDBConObject(); //設定資料庫連結
                TempData = DB_Search.GetGlobalParam();
                //Update
                DB_Update = new UpdateSQL_Service();
                DB_Update.DataConnection = DB_Con.GetDBConObject(); //設定資料庫連結
                if (DB_Update.Update(OperationString, TempData) != 1)
                    Result = "<:~ XmlVersion 更新時，發生問題 ~:>";
                DB_Con.CloseConnection();
            }
            return "<:~ All of task is Complete ~:>";
        }

        /*
         * Xml 模組與 SQL的欄位名稱 
         */
        private XDocument Xml_Part()
        {
            Xml_Operation = new ProcessXml();
            //1. 檔案路徑 3.檔案名稱
            Xml_Operation.InitXmlPath(OperationMapTable.GetXmlPath(OperationString), OperationMapTable.GetXmlInfo(OperationString) + "Model\\", OperationString);
            //SQL_List
            Xml_Operation.CulomnNameList = OperationMapTable.GetSQLArray(OperationString, OperationString + RepositoryOperation);
            //開檔
            return Xml_Operation.OpenXml();
        }

        /*
         * Step 1 : 查出資料庫欄位的List
         * Step 2 : 利用資料庫名稱欄位去取得資料庫資料
         */
        private DataSet SQL_SearchPart()
        {
            DB_Search = new SearchSQL_Service();
            //設定資料庫連結
            DB_Search.DataConnection = DB_Con.GetDBConObject();
            //找出查詢規則
            DB_Search.Search_Table_Name_List = OperationMapTable.GetSQLArray(OperationString, OperationString + RepositoryOperation);
            //收到解答
            return DB_Search.Search_operation();
        }
    }
}