using LongTermCare_Xml_.Models.DTO_Models;
using LongTermCare_Xml_.Models.Setting;
using LongTermCare_Xml_.Models.SQL_Operation;
using LongTermCare_Xml_.Models.Xml_Operation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.OleDb;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace LongTermCare_Xml_.Models.Process_Xml
{
    public class ProcessXml
    {
        private Connection_DataBase DB_Con;
        private SearchSQL_Service DB_Search;
        private UpdateSQL_Service DB_Update;
        private DBOper_DTO DB_Oper = null;
        private BaseDBOpertion<DBOper_DTO> DB_Method;
        //Column_Name Dictionary
        private Dictionary<string, List<string>> ColumnName_Dic = new Dictionary<string, List<string>>();

        //更新列表Xml
        private XDocument PatientPCDList = null;
        //Model File Streams
        public XDocument XmlDoc { get; set; }
        //SQLDataSet
        public DataSet DB_DataSet { get; set; }

        //資料表欄位名稱的List
        public List<string> CulomnNameList { get; set; }
        //資料表名稱的List
        public List<XElement> XmlModelList { get; set; }
        //主要金鑰的List
        private List<string> Primarykey = new List<string>();

        //Xml檔案路徑
        private string XmlPath { get; set; }
        private string ModelPath { get; set; }
        private string XmlFileName { get; set; }

        public ProcessXml() {
            DB_Oper = new DBOper_DTO();
            DB_Method = new BaseDBOpertion<DBOper_DTO>(DB_Oper);
            DB_Method.SettingConnectionString(ref DB_Oper);
        }
        ~ProcessXml()
        {
            System.Diagnostics.Trace.WriteLine("ProcessXml is clear");
        }

        /*
         * 初始化Xml路徑
         */
        public void InitXmlPath(string XmlBasicPath, string ModelPath, string XmlFileName)
        {
            this.XmlPath = XmlBasicPath;
            this.ModelPath = ModelPath;
            this.XmlFileName = XmlFileName;
        }

        /*
         * 更新Xml 
         */
        public string CreateXml()
        {
            InitSystem();
            //新XmlList檔
            PatientPCDList = new XDocument();
            XElement ResultEl = new XElement(XmlDoc.Root.Name.LocalName);

            //假如只有一層
            if (DB_DataSet.Tables.Count == 1)
                OnceLayerMethod(ResultEl);
            else//有兩層
                ManyLayerMethod(ResultEl);
            PatientPCDList.Add(ResultEl);
            WriteXml();
            return PatientPCDList.ToString();
        }

        private void OnceLayerMethod(XElement ResultEl)
        {
            int CurrentCount = 0, RowCount = 0, LayerCount = -1;
            string PrimaryKey = "";
            DataRow[] TempRow;
            XElement Temp = null;
            TempRow = GetRows(PrimaryKey, LayerCount);
            RowCount = GetRowsCount(TempRow);
            do
            {
                Temp = XmlResult(AddNewPoint(TempRow[CurrentCount], LayerCount + 1, false), Temp);
                AddXElement(ResultEl, Temp, LayerCount + 1);
                CurrentCount++;
            } while (CurrentCount != RowCount);
        }

        private void ManyLayerMethod(XElement ResultEl)
        {
            int CurrentCount = 0, PrimaryKeyIndex = 0, RowCount = 0, LayerCount = -1;
            string PrimaryKey = "", ParentPrimaryKey = "";
            DataRow[] TempRow = null;
            XElement Temp = null;
            //紀錄狀態的堆疊
            Stack<State> SqlModelState = new Stack<State>();
            //初始化Root節點
            State InitState = new State(-1, -1, "", "");
            SqlModelState.Push(InitState);
            //建立列表
            do
            {
                Temp = null;
                //判斷是否有子節點，有的以子節點 0 開始
                if (CheckChildLayer(PrimaryKey, LayerCount))
                {
                    CurrentCount = 0;
                    ParentPrimaryKey = PrimaryKey;
                    //找出本點的有幾個Count
                    TempRow = GetRows(PrimaryKey, LayerCount);
                    RowCount = GetRowsCount(TempRow);
                    //子節點超過1個，把其它子節點加入Stack
                    if (RowCount > 1)
                    {
                        //初始化變數
                        int TempLayerCount = 0, TempCurrentCount = 0;
                        string TempPrimaryKey = "", TempParentPrimaryKey = "";
                        State TempState = null;
                        for (int Count = RowCount - 1; Count > 0; Count--)
                        {
                            TempState = null;
                            TempLayerCount = LayerCount + 1;
                            TempCurrentCount = Count;
                            PrimaryKeyIndex = GetPrimaryKeyIndex(TempLayerCount);
                            TempPrimaryKey = TempRow[TempCurrentCount].ItemArray[PrimaryKeyIndex].ToString();
                            TempParentPrimaryKey = ParentPrimaryKey;
                            TempState = new State(TempCurrentCount, TempLayerCount, TempPrimaryKey, TempParentPrimaryKey);
                            SqlModelState.Push(TempState);
                        }
                    }
                    //下一個子節點
                    LayerCount++;
                    PrimaryKeyIndex = GetPrimaryKeyIndex(LayerCount);
                    PrimaryKey = TempRow[CurrentCount].ItemArray[PrimaryKeyIndex].ToString();
                }
                //沒有子節點，把Stack中的紀錄的點拿出來
                else
                {
                    //Study沒有Series的狀況
                    if (LayerCount == 1)
                    {
                        Temp = null;
                        DataSet SearchDBSet = null;
                        DB_Con = new Connection_DataBase(DB_Oper);
                        if (DB_Con.IsDBConnection())//判斷資料庫連結
                        {
                            //Search
                            DB_Search = new SearchSQL_Service();
                            DB_Search.DataConnection = DB_Con.GetDBConObject(); //設定資料庫連結
                            SearchDBSet = DB_Search.GetGlobalParam();
                            try
                            {
                                var Adapter = new OleDbDataAdapter();
                                OleDbCommandBuilder builder = new OleDbCommandBuilder(Adapter);//關聯DataSet和資料庫的操作的，必不可少
                                Adapter.SelectCommand = new OleDbCommand("Select * from Series", DB_Con.GetDBConObject());
                                DataTable DBTable = new DataTable();
                                Adapter.Fill(DBTable);
                                DataRow DBRow = DBTable.NewRow();
                                //填充欄位資訊
                                DBRow[0] = DateTime.Now.ToString("yyyyMMdd");
                                DBRow[1] = DateTime.Now.ToString("HHmm");
                                DBRow[2] = null;
                                DBRow[3] = PrimaryKey;
                                DBRow[4] = SearchDBSet.Tables[0].Rows[0].ItemArray[4].ToString().Trim() + ".0." + SearchDBSet.Tables[0].Rows[0].ItemArray[0];
                                DBRow[5] = SearchDBSet.Tables[0].Rows[0].ItemArray[6];
                                SearchDBSet.Tables[0].Rows[0].ItemArray[0] = int.Parse(SearchDBSet.Tables[0].Rows[0].ItemArray[0].ToString()) + 1;
                                SearchDBSet.Tables[0].Rows[0].ItemArray[6] = int.Parse(SearchDBSet.Tables[0].Rows[0].ItemArray[6].ToString()) + 1;
                                //把助比新資料存入DataSet中
                                DBTable.Rows.Add(DBRow);
                                Adapter.InsertCommand = builder.GetInsertCommand();
                                Adapter.Update(DBTable);
                                DBTable.AcceptChanges();
                                DB_Con.CloseConnection();
                                Temp = XmlResult(AddNewPoint(DBRow, 2, true), Temp);
                                AddXElement(ResultEl, Temp, 2);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            finally
                            {
                                DB_Con = new Connection_DataBase (DB_Oper);
                                DB_Update = new UpdateSQL_Service();
                                DB_Update.DataConnection = DB_Con.GetDBConObject(); //設定資料庫連結
                                DB_Update.Update("Series", SearchDBSet);
                                DB_Con.CloseConnection();
                            }
                        }
                    }
                    //下一個點拿出來處理
                    State TempsState = null;
                    TempsState = SqlModelState.Pop();
                    LayerCount = TempsState.GetLayerCount();
                    PrimaryKey = TempsState.GetPrimaryKey();
                    CurrentCount = TempsState.GetCurrentCount();
                    //結束條件
                    if (CurrentCount == -1 && LayerCount == -1)
                        break;
                    //找出本點的有幾個Count
                    TempRow = GetRows(ParentPrimaryKey, LayerCount - 1);
                }
                Temp = XmlResult(AddNewPoint(TempRow[CurrentCount], LayerCount, true), Temp);
                AddXElement(ResultEl, Temp, LayerCount);
            } while (SqlModelState.Count != 0);
        }

        /*
         * 串接Xml檔案
         */
        private void AddXElement(XElement Root, XElement element, int LayerCount)
        {
            XElement temp = (XElement)Root.LastNode;
            for (int i = 0; i < LayerCount; i++)
            {
                temp = (XElement)temp.LastNode;
            }
            if (temp == null)
                Root.Add(element);
            else
                temp.AddAfterSelf(element);
        }

        /*
         * 初始化 各個表單
         */
        private void InitSystem()
        {
            //建立 SQL 欄位名稱
            CreateColumnName_Dic();
            //建立 Xml Model
            CreateXmlModel();
            //建立 Primary Key 
            CreatePrimaryKey();
        }

        /*
         * 資料庫 Mapping Xml欄位
         */
        private XElement AddNewPoint(DataRow Data, int LayerCount, bool Judge_Value)
        {
            // LINQ to XML query         
            XElement XmlMode = null, element = null;
            int Count = 0;
            List<string> ColumnNames = GetColumnName(CulomnNameList[LayerCount]);
            XmlMode = XmlModelList[LayerCount];
            foreach (string ColumnName in ColumnNames)
            {
                //找到值得時候加入
                if ((element = SearchElement(XmlMode, ColumnName, Judge_Value)) != null)
                    element.SetValue(Data[Count]);
                Count++;
            }
            return XmlMode;
        }

        /*
         * 查詢 XElement  
         */
        private XElement SearchElement(XElement Node, string Column_Name, bool Judge_Value)
        {
            XElement CurrentElement = null;
            try
            {
                if (Judge_Value)
                    CurrentElement = Node.Elements().FirstOrDefault(el => el.Attribute("FieldName").Value == Column_Name);
                else
                    CurrentElement = Node.Elements().FirstOrDefault(el => el.Name == Column_Name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            return CurrentElement;
        }

        /*
         *創建欄位名稱列表 
         */
        private void CreateColumnName_Dic()
        {
            for (int TableCount = 0; TableCount < DB_DataSet.Tables.Count; TableCount++)
            {
                List<string> ColumnName = new List<string>();
                //欄位名稱
                foreach (DataColumn dc in DB_DataSet.Tables[TableCount].Columns)
                    ColumnName.Add(dc.ColumnName);
                ColumnName_Dic.Add(CulomnNameList[TableCount], ColumnName);
            }
        }

        /*
         * 創建 Primary Key 表
         */
        private void CreatePrimaryKey()
        {
            //查出PrimaryKey
            for (int DB_Count = 0; DB_Count < DB_DataSet.Tables.Count; DB_Count++)
            {
                DataColumn[] key = DB_DataSet.Tables[DB_Count].PrimaryKey;
                Primarykey.Add(key[0].ToString());
            }
        }

        /*
         *建立Xml Model 
         */
        private List<XElement> CreateXmlModel()
        {
            int LayoutCount = 0;
            XmlModelList = new List<XElement>();
            IEnumerable<XElement> TempModel = XmlDoc.Root.Elements();
            //  if (TempModel.Last().Elements())
            XElement temp2;
            while (TempModel.Last().Elements().Any())
            {
                string TagName = null;
                if (LayoutCount == 0)
                    TagName = TempModel.First().Name.LocalName;
                else
                    TagName = TempModel.Last().Name.LocalName;
                temp2 = new XElement(TagName);
                XElement temp = null;
                IEnumerable<XElement> TempModels = TempModel.FirstOrDefault(el => el.Name == TagName).Elements();
                for (int i = 0; i < TempModels.Count(); i++)
                {
                    int check = 0;
                    if (i == TempModels.Count() - 1)
                    {
                        if (TempModels.Cast<XElement>().ElementAt(i).Elements().Any())
                            check = 1;
                        else
                            temp = TempModels.Cast<XElement>().ElementAt(i);
                    }
                    else
                        temp = TempModels.Cast<XElement>().ElementAt(i);
                    if (check != 1)
                        temp2.Add(temp);
                }
                XmlModelList.Add(temp2);
                LayoutCount++;
                TempModel = TempModel.Last().Elements();
            }
            return XmlModelList;
        }

        /*
         * 取得Rows的
         */
        private DataRow[] GetRows(string PrimaryKey, int LayerCount)
        {
            //第一層，沒有父節點
            if (PrimaryKey == "" || LayerCount == -1)
                return DB_DataSet.Tables[LayerCount + 1].Rows.Cast<DataRow>().ToArray();
            else
            {
                //取得這一層的Primary Key
                string PrimaryKeyString = GetPrimarykey(LayerCount);
                //利用PrimaryKey 找出關聯資料   
                return DB_DataSet.Tables[LayerCount + 1].Select(PrimaryKeyString + "='" + PrimaryKey + "'");
            }
        }

        /*
         * 取得Row的Count
         */
        private int GetRowsCount(DataRow[] TempRow)
        {
            int Count = TempRow.Count();
            return Count;
        }

        /*
         *回傳 Primary Key 的 Index  
         */
        private int GetPrimaryKeyIndex(int LayerCount)
        {
            //利用PrimaryKey 找出PrimaryKey的Index
            string PrimaryKey = GetPrimarykey(LayerCount);
            return DB_DataSet.Tables[LayerCount].Columns.IndexOf(PrimaryKey);
        }

        /*
         * 取得資料欄位名稱
         */
        private List<string> GetColumnName(string key)
        {
            List<string> value;
            ColumnName_Dic.TryGetValue(key, out value);
            return value;
        }

        /*
         * 找出主要金鑰是哪一個
         */
        private string GetPrimarykey(int LayerCount)
        {
            return Primarykey[LayerCount];
        }

        /*
         * 看看有沒有子節點
         */
        private bool CheckChildLayer(string PrimaryKey, int LayerCount)
        {
            //已經到底了沒有資料
            if (LayerCount == DB_DataSet.Tables.Count - 1)
                return false;
            else
            {
                DataRow[] Temp = GetRows(PrimaryKey, LayerCount);
                if (Temp.Count() > 0)
                    return true;
                else
                    return false;
            }
        }

        /*
         * 建立新的XElement的點
         */
        private XElement XmlResult(XElement Nodes, XElement TempModel)
        {
            XElement tempel;
            string TagName = Nodes.Name.LocalName;
            TempModel = new XElement(TagName);
            IEnumerable<XElement> TempModels = Nodes.Elements();
            for (int i = 0; i < TempModels.Count(); i++)
            {
                tempel = TempModels.Cast<XElement>().ElementAt(i);
                TempModel.Add(tempel);
            }
            return TempModel;
        }

        /*
               * 開檔操作
               * 回傳 XDocument
               * 需要設定    
               * public string XmlPath { get; set; }
               * public string XmlFileName { get; set; }
               */
        public XDocument OpenXml()
        {
            XDocument Doc = null;
            string Path = XmlPath + ModelPath + XmlFileName + ".xml";
            try
            {
                Doc = XDocument.Load(Path.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return Doc;
        }

        /*
       * 寫檔操作
       * 回傳 int  
       * 0 error;
       * 1 okay;
       */
        public int WriteXml()
        {
            //把做好的檔案儲存
            ModelPath = "Update_Xml\\";
            string Path = XmlPath + ModelPath + XmlFileName + ".xml";
            PatientPCDList.Save(Path);
            return 1;
        }

    }
}