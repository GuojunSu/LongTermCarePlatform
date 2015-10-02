using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Web.Configuration;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.IO;

/// <summary>
/// search 的摘要描述
/// </summary>
public class search
{
	public search()
	{
		//
		// TODO: 在這裡新增建構函式邏輯
		//
	}
    
    SqlCommand sqlCmd1;
    SqlDataReader read1, read2;
    string Terms;
    
    string[] QFieldName = new string[50], Qvalue = new string[25], QVR = new string[25] ,Tag = new string[50], FieldName = new string[50];
    int a = 1;
    public int ProcessXML(XDocument file,string userid)
    {
        string level;
        SqlCommand sqlCmd1;
        SqlDataReader read1;
        SqlConnection sqlCon = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        sqlCon.Open();

        
        if (userid == null)
            userid = "PatientFolder";
        string FolderPath = @"D:\C sharp code\LongTermCare(Xml)\Search\Query_Xml\" + userid;
        DirectoryInfo DIFO = new DirectoryInfo(FolderPath);//判斷資料夾存在否
        if (!DIFO.Exists)
        {
            DIFO.Create();
        }

        level = file.Root.Name.ToString();
        IEnumerable<XElement> list1 = file.XPathSelectElements(level);//root node
        int Querycount = 0;
        foreach (XElement item in list1.Elements())
        {//get fieldname and value
            if (item.Value.ToString() != "")
            {
                QFieldName[Querycount] = item.Attribute("FieldName").Value.ToString().Replace(" ", "");
                QVR[Querycount] = item.Attribute("VR").Value.ToString();
                Qvalue[Querycount++] = item.Value.ToString();
            }
        }
        int checkwhere = 0;
        for (int i = 0; i < Querycount; i++)//saerch tablename then combine SQL
        {
            sqlCmd1 = new SqlCommand("select DISTINCT TableName from TableFields where FieldName='" + QFieldName[i] + "'", sqlCon);
            read1 = sqlCmd1.ExecuteReader();
            read1.Read();
            if (Qvalue[i] != "*")
            {
                if (++checkwhere == 1)
                    Terms = "where" + ' ' + Terms;
                Terms += read1["TableName"].ToString() + "." + QFieldName[i];
                if ((QVR[i] == "DA" || QVR[i] == "TM") && Qvalue[i].Length == 9)
                    Terms += Qvalue[i];
                else if ((QVR[i] == "DA" || QVR[i] == "TM") && Qvalue[i].Length > 9)
                {
                    string[] strs = Qvalue[i].Split('-');
                    Terms += ">" + strs[0] + " and " + read1["TableName"].ToString() + "." + QFieldName[i] + "<" + strs[1];
                }
                else
                    Terms += "=" + "'" + Qvalue[i] + "'";
                if (i < Querycount - 1)
                    Terms += ' ' + "and ";
            }
        }
        return Query(level, userid);
    }
    public int Query(string level,string userid)//Search db
    {
        DirectoryInfo DIFO1 = new DirectoryInfo(@"D:\C sharp code\LongTermCare(Xml)\Search\Search_Xml\" + userid);//userid
        DIFO1.Create();
        SqlConnection sqlCon = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        sqlCon.Open();
        switch (level)
        {//read2:Tag and FieldName
            case "PATIENT":
                {
                    sqlCmd1 = new SqlCommand("select * from Patient " + Terms + "", sqlCon);
                    read1 = sqlCmd1.ExecuteReader();
                    sqlCmd1 = new SqlCommand("select DISTINCT tag ,FieldName from TableFields where TableName='PATIENT' order by FieldName", sqlCon);
                    read2 = sqlCmd1.ExecuteReader();
                    break;
                }
            case "STUDY":
                {
                    sqlCmd1 = new SqlCommand("select * from Patient,Study " + Terms + "and Study.PatientID=Patient.PatientID", sqlCon);//"select * from Patient,Study '" + Terms + "'" + "and Study.PatientID=Patient.PatientID"
                    read1 = sqlCmd1.ExecuteReader();
                    sqlCmd1 = new SqlCommand("select DISTINCT tag ,FieldName from TableFields where TableName='PATIENT' or TableName='STUDY' order by FieldName", sqlCon);
                    read2 = sqlCmd1.ExecuteReader();
                    break;
                }
            case "SERIES":
                {
                    sqlCmd1 = new SqlCommand("select * from Patient,Study,Series  '" + Terms + "'" + "and  Series.StudyInstanceUID=Study.StudyInstanceUID and Study.PatientID=Patient.PatientID", sqlCon);
                    read1 = sqlCmd1.ExecuteReader();
                    sqlCmd1 = new SqlCommand("select DISTINCT tag ,FieldName from TableFields where TableName='PATIENT' or TableName='STUDY' or TableName='SERIES' order by FieldName", sqlCon);
                    read2 = sqlCmd1.ExecuteReader();
                    break;
                }
            case "SOPInstance":
                {
                    string a = "select distinct * from Patient,Study,Series,SOPInstance  " + Terms + "and SOPInstance.SeriesInstanceUID=Series.SeriesInstanceUID and Series.StudyInstanceUID=Study.StudyInstanceUID and Study.PatientID=Patient.PatientID";
                    sqlCmd1 = new SqlCommand(a, sqlCon);
                    read1 = sqlCmd1.ExecuteReader();
                    sqlCmd1 = new SqlCommand("select DISTINCT tag ,FieldName from TableFields where TableName='PATIENT' or TableName='STUDY' or TableName='SERIES' or TableName='SOPInstance' order by FieldName", sqlCon);
                    read2 = sqlCmd1.ExecuteReader();
                    break;
                }
        }
        int i = 0;

        if (read1.HasRows != false)
        {
            while (read2.Read())
            {
                FieldName[i] = read2["FieldName"].ToString();
                Tag[i++] = read2["tag"].ToString();
            }

            while (read1.Read())
            {
                XElement xdoc = XElement.Load(@"D:\C sharp code\LongTermCare(Xml)\Search\SearchModel\" + level + "XML-1.xml");
                for (int j = 0; j < i; j++)
                {
                    IEnumerable<XElement> tests =
                    from el in xdoc.Elements("Tag" + Tag[j])
                    select el;
                    if (read1[FieldName[j]].ToString() != null)//set search result to xml
                    {
                        foreach (XElement el in tests)
                        {
                            el.SetValue(read1[FieldName[j]].ToString().Trim());
                        }
                    }
                }
                xdoc.Save(@"D:\C sharp code\LongTermCare(Xml)\Search\Query_Xml\" + userid + @"\" + a + ".xml");//save result file
                a++;
            }
        }
        read1.Close();
        read1.Dispose();
        read2.Close();
        read2.Dispose();
        sqlCmd1.Cancel();
        sqlCmd1.Dispose();
        sqlCon.Close();
        sqlCon.Dispose();
        return a-1;
    }
}