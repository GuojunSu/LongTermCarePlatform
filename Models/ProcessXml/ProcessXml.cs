using LongTermCare_Xml_.Models.Setting;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Xml;
using System.Xml.Linq;


namespace LongTermCare_Xml_.Models.ProcessXml
{
    public class ProcessXml
    {
        List<string> KeyList = new List<string>();
        string[] PrimaryKey = new string[4] { "PatientID", "StudyInstanceUID", "SeriesInstanceUID", "SOPInstanceUID" };
        private XDocument PCDXml { get; set; }
        private XDocument SearchXml { get; set; }
        private SettingInfo Path_Info { get; set; }
        private InitXml XmlInCacher { get; set; }
        protected Logger Log { get; private set; }
        //建構
        public ProcessXml(SettingInfo Info)
        {
            Path_Info = Info;
        }
        //建構
        public ProcessXml(SettingInfo Path_Info, InitXml XmlDoc)
        {
            this.Path_Info = Path_Info;
            this.XmlInCacher = XmlDoc;
        }
        //查詢操作
        public int SearchOperation(XmlDocument Search, string account)
        {
            int FileCount = 0;
            //轉換成XDocument
            this.SearchXml = XDocument.Parse(Search.OuterXml);
            //做LogRecord
            // alternatively you can call the Log() method 
            // and pass log level as the parameter.
            Log.Log(LogLevel.Info, "Sample informational message");
            //把結果給學弟
            try
            {
                search SearchFun = new search();
                FileCount = SearchFun.ProcessXML(SearchXml, account);
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
            Log = LogManager.GetCurrentClassLogger();
            //把XmlDocument轉換成XDocument
            this.PCDXml = XDocument.Parse(PCD.OuterXml);
            //負責填質的部分
            FillFrontSection(PCDXml);
            FillLaterSection(PCDXml);
            XDocument FullInsertXml = FinalSection();
            //把結果給學弟
            // alternatively you can call the Log() method 
            // and pass log level as the parameter.
            Log.Log(LogLevel.Info, "Sample informational message");
            try
            {
                insert InsertFun = new insert();
                InsertFun.ProcessXML(FullInsertXml);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException());
            }
            return 200;
        }
        //處理Patient 和 Study
        public void FillFrontSection(XDocument Doc)
        {
            int i = 0, check = 0;
            foreach (XElement element in PCDXml.Root.Elements())
            {
                int count = 0;
                XDocument XmlDoc = null;
                string FieldName = element.Attribute("FieldName").Value;
                if (FieldName.Equals(PrimaryKey[i]))
                {
                    switch (FieldName)
                    {
                        case "PatientID":
                            KeyList.Add(FieldName);
                            KeyList.Add(element.Value);
                            break;
                        case "StudyInstanceUID":
                            KeyList.Add(FieldName);
                            KeyList.Add(element.Value);
                            //STUDYXML Open
                            XmlDoc = XmlInCacher.StudyXml;
                            //補齊資料
                            foreach (XElement Element in XmlDoc.Root.Elements())
                            {
                                string Target = Element.Attribute("FieldName").Value;
                                if (Target.Equals(KeyList[count]))
                                {
                                    count++;
                                    Element.SetValue(KeyList[count]);
                                    if (KeyList.Count - 1 != count)
                                        count++;
                                    else
                                        break;
                                }
                            }
                            //寫出數據
                            WriteXml(XmlDoc, "Search", Path_Info.SearchPath, "STUDYXML-1.xml");
                            //處理完初始化
                            XmlInCacher.StudyXml = InitCacher(XmlDoc);
                            break;
                    }
                    if (PrimaryKey.Length - 1 != i)
                        i++;
                }
                //把剩下的值補齊
                if (check != 0)
                {
                    KeyList.Add(FieldName);
                    KeyList.Add(element.Value);
                }
                if (FieldName.Equals("ReferringPhysiciansName"))
                    check++;
                else
                    continue;
            }
        }
        //處理Series 和 SOPInstance
        public void FillLaterSection(XDocument Doc)
        {
            //判別資料庫連線
            PCDEntities Context = null;
            string UID = null;
            int SeriesInstanceCount = 0, SOPInstanceCount = 0, PatientCount = 0, StudyInstanceCount = 0;
            //取得資料庫全域變數
            try
            {
                Context = new PCDEntities();
                UID = Context.Set<GlobalParam>().Select(Global => Global.UID).First();
                SeriesInstanceCount = Context.Set<GlobalParam>().Select(Global => Global.SeriesInstanceCount).First();
                SOPInstanceCount = Context.Set<GlobalParam>().Select(Global => Global.SOPInstanceCount).First();
                PatientCount = Context.Set<GlobalParam>().Select(Global => Global.PatientCount).First();
                StudyInstanceCount = Context.Set<GlobalParam>().Select(Global => Global.StudyInstanceCount).First();
            }
            catch (Exception e)
            {
                throw e.GetBaseException();
            }

            int count = 4, check = 0;
            //初始化Insert.xml
            string ReadFileName = "Insert.xml", Operation = "Insert";
            XDocument Xmls = XmlInCacher.InsertXml;
            foreach (XElement element in Xmls.Root.Elements())
            {
                string FieldName = element.Attribute("FieldName").Value;
                if (check != 0 && KeyList[count].Equals(FieldName))
                {
                    count++;
                    if (KeyList[count].Equals(""))
                    {
                        DateTime currentTime = new DateTime();
                        currentTime = DateTime.Now;
                        //新增UUID加入Element
                        if (FieldName.Equals("SeriesInstanceUID") || FieldName.Equals("SOPInstanceUID"))
                        {
                            string UUID = null;
                            if (FieldName.Equals("SeriesInstanceUID"))
                            {
                                UUID = UID.Trim() + "." + SeriesInstanceCount;
                                SeriesInstanceCount++;
                            }
                            else
                            {
                                UUID = UID.Trim() + "." + SOPInstanceCount;
                                SOPInstanceCount++;
                            }
                            element.SetValue(UUID);
                        }
                        else if (FieldName.Equals("SeriesDate"))
                        {
                            //取當前年/月/日  
                            string Date = currentTime.ToString("yyyyMMdd");
                            element.SetValue(Date);
                        }
                        else if (FieldName.Equals("SeriesTime"))
                        {
                            //取當時分秒  
                            string Time = currentTime.ToString("HHmmss");
                            element.SetValue(Time);
                        }
                    }
                    if (!KeyList[count].Equals(""))
                    {
                        if (FieldName.Equals("DeviceRecordResult") || FieldName.Equals("ContributingEquipmentSequence"))
                        {
                            XElement Result = PCDXml.Root.Elements().Where(Tag => Tag.Attribute("FieldName").Value == FieldName).First();
                            foreach (XElement subelement in Result.Elements())
                                element.SetElementValue(subelement.Name, subelement.Value);
                            Result = null;
                        }
                        else if (!FieldName.Equals("DeviceRecordResult") && !FieldName.Equals("ContributingEquipmentSequence"))
                            element.SetValue(KeyList[count]);
                    }
                    count++;
                }
                if (FieldName.Equals("ReferringPhysiciansName"))
                    check++;
                else
                    continue;
            }
            WriteXml(Xmls, Operation, Path_Info.InsertPath, ReadFileName);
            //處理完初始化
            XmlInCacher.InsertXml = InitCacher(Xmls);
            //更新資料庫有使用Transaction
            using (var dbContextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var result = Context.GlobalParam.SingleOrDefault(GP => GP.PKey == 0);
                    if (result != null)
                    {
                        result.SeriesInstanceCount = SeriesInstanceCount;
                        result.SOPInstanceCount = SOPInstanceCount;
                        Context.SaveChanges();
                    }
                    dbContextTransaction.Commit();
                }
                catch (UpdateException ex)
                {
                    //更新失敗rollback
                    dbContextTransaction.Rollback();
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        //最後階段
        public XDocument FinalSection()
        {
            XDocument SearchXml = OpenXml("SearchTemp", Path_Info.SearchPath, "STUDYXML-1.xml");
            //呼叫查詢
            search SearchFun = new search();
            int FileCount = SearchFun.ProcessXML(SearchXml, "ServerTemp");

            //進行資料新增
            string Operation = "InsertQuery", ReadFileName = "Insert.xml";
            XDocument QueryXml = OpenXml("Query", Path_Info.SearchPath, FileCount.ToString().Trim());
            //Insert Xml
            XDocument Doc = OpenXml(Operation, Path_Info.InsertPath, ReadFileName);
            foreach (XElement element in Doc.Root.Elements())
            {
                string FieldName = element.Attribute("FieldName").Value;
                foreach (XElement Element in QueryXml.Root.Elements())
                {
                    string QueryName = Element.Attribute("FieldName").Value;
                    if (FieldName.Equals(QueryName))
                    {
                        element.SetValue(Element.Value);
                        break;
                    }
                }
                if (FieldName.Equals("SeriesInstanceUID"))
                    break;
            }
            WriteXml(Doc, Operation, Path_Info.InsertPath, ReadFileName);
            return Doc;
        }
        //開啟Xml模板檔案
        public XDocument OpenXml(string Operation, string OperationPath, string XmlFileName)
        {
            XDocument Doc = null;
            string Path = null;
            if (Operation.Equals("Search"))
                Path = OperationPath + "SearchModel\\" + XmlFileName;
            if (Operation.Equals("SearchTemp"))
                Path = OperationPath + "Search_Xml\\" + XmlFileName;
            if (Operation.Equals("Query"))
                Path = OperationPath + "Query_Xml\\ServerTemp\\" + XmlFileName + ".Xml";
            else if (Operation.Equals("Insert"))
                Path = OperationPath + "InsertModel\\" + XmlFileName;
            else if (Operation.Equals("InsertQuery"))
                Path = OperationPath + "Insert_Xml\\" + XmlFileName;
            else if (Operation.Equals("Update"))
                Path = @"D:\C sharp code\LongTermCare(Xml)\Update\" + XmlFileName;
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
        //寫出Xml資訊
        public int WriteXml(XDocument Doc, string Operation, string OperationPath, string XmlFileName)
        {
            string Path = null;
            if (Operation.Equals("Search"))
                Path = OperationPath + "Search_Xml\\" + XmlFileName;
            else
                Path = OperationPath + "Insert_Xml\\" + XmlFileName;
            Doc.Save(Path);
            return 200;
        }
        //初始化記憶體中資訊
        public XDocument InitCacher(XDocument Xml)
        {
            foreach (XElement ele in Xml.Root.Elements())
            {
                ele.SetValue("");
            }
            return Xml;
        }
    }
}