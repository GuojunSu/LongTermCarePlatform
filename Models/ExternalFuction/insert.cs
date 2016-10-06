using System;
using System.Data;
using System.Configuration;
using System.Collections;
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
using System.Web.Security;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Data.OleDb;

/// <summary>
/// insert 的摘要描述
/// </summary>
public class insert
{
    /*1.QueryString filename & filefolder.
     *2.Select DB Field.
     *3.ProcessXML.
     *4.Duplicate Data then INSERT TO DB.
     ****
     */
    string[] value = new string[100], tag = new string[100], FieldName = new string[100];
    int Patientcount = 0, Studycount = 0, Seriescount = 0, Imagecount = 0,Index_PID,Index_STUID,Index_SERUID,Index_SOPUID;
	public insert()
	{
		//
		// TODO: 在這裡新增建構函式邏輯
		//
	}
    public int ProcessXML(XDocument file,string DBpath)
    {
        SearchFieldName(DBpath);
        IEnumerable<XElement> list1 = file.XPathSelectElements("//PCD");//root node

        foreach (XElement item in list1.Elements())
        {
            string XMLTagName = item.Name.ToString(), Tagname = "";
            for (int i = 3; i <= 10; i++)//catch tag
                Tagname += XMLTagName[i];
            for (int index = 0; index < Imagecount; index++)
            {
                if (Tagname == tag[index])
                {
                    if (Tagname == "0027a001")//DeviceRecordResult
                    {
                        value[index] = item.FirstNode.ToString();
                        if (item.FirstNode.NextNode!=null)
                        {
                            value[index] += item.FirstNode.NextNode.ToString();
                            value[index] += item.LastNode.ToString();
                        }
                    }
                    else if (Tagname == "0018a001")//ContributingEquipmentSequence
                    {
                        value[index] = item.FirstNode.ToString();
                        value[index] += item.FirstNode.NextNode.ToString();
                        value[index] += item.FirstNode.NextNode.NextNode.ToString();
                        value[index] += item.LastNode.ToString();
                    }
                    else if (item.Value.ToString() != "")
                        value[index] = item.Value.ToString().Replace("'", "''");//replace ' to ''
                    else
                        value[index] = "null";//has no value
                }
            }
        }
        for (int i = 0; i < Imagecount; i++)
            if (value[i] == "" || value[i] ==null)
                value[i] = "null";
        return CheckandInsert(DBpath, file);
    }
    public void SearchFieldName(string DBpath)
    {
       // SqlConnection sqlCon = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        SqlConnection sqlCon = new SqlConnection(DBpath);
        sqlCon.Open();
        //ALL Field Name
        SqlCommand sqlCmd1 = new SqlCommand("select DISTINCT tag as a,FieldName as b  from TableFields where TableFields.FieldName IN (select name from syscolumns where id=object_id('Patient'))", sqlCon);
        SqlCommand sqlCmd2 = new SqlCommand("select DISTINCT tag as a,FieldName as b from TableFields where TableFields.FieldName IN (select name from syscolumns where id=object_id('Study'))", sqlCon);
        SqlCommand sqlCmd3 = new SqlCommand("select DISTINCT tag as a,FieldName as b from TableFields where TableFields.FieldName IN (select name from syscolumns where id=object_id('Series'))", sqlCon);
        SqlCommand sqlCmd4 = new SqlCommand("select DISTINCT tag as a,FieldName as b from TableFields where TableFields.FieldName IN (select name from syscolumns where id=object_id('SOPInstance'))", sqlCon);

        SqlDataReader read1 = sqlCmd1.ExecuteReader();
        SqlDataReader read2 = sqlCmd2.ExecuteReader();
        SqlDataReader read3 = sqlCmd3.ExecuteReader();
        SqlDataReader read4 = sqlCmd4.ExecuteReader();

        for (int i = 0; read1.Read(); i++)//4 level table copy to array
        {
           
            tag[i] = read1["a"].ToString();
            FieldName[i] = read1["b"].ToString();
            Patientcount = i + 1;
            if (tag[i] == "00100020")
                Index_PID = i;
        }
        for (int i = Patientcount; read2.Read(); i++)
        {
            tag[i] = read2["a"].ToString();
            FieldName[i] = read2["b"].ToString();
            Studycount = i + 1;
            if (tag[i] == "0020000d")
                Index_STUID = i;
        }
        for (int i = Studycount; read3.Read(); i++)
        {
            tag[i] = read3["a"].ToString();
            FieldName[i] = read3["b"].ToString();
            Seriescount = i + 1;
            if (tag[i] == "0020000e")
                Index_SERUID = i;
        }
        for (int i = Seriescount; read4.Read(); i++)
        {
            tag[i] = read4["a"].ToString();
            FieldName[i] = read4["b"].ToString();
            Imagecount = i + 1;
            if (tag[i] == "00080018")
                Index_SOPUID = i;
        }
        sqlCmd1.Cancel();
        sqlCmd2.Cancel();
        sqlCmd3.Cancel();
        sqlCmd4.Cancel();
        read1.Close();
        read2.Close();
        read3.Close();
        read4.Close();
        sqlCon.Close();
        sqlCon.Dispose();
    }
    public int CheckandInsert(string DBpath, XDocument file)
    {
        SqlConnection sqlCon = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        sqlCon.Open();
        //Check Duplicate
        SqlCommand sqlCmd1 = new SqlCommand("select Count(PatientID) as a from Patient where PatientID ='" + value[Index_PID] + "'", sqlCon);
        SqlCommand sqlCmd2 = new SqlCommand("select Count(StudyInstanceUID) as a from Study where StudyInstanceUID ='" + value[Index_STUID] + "'", sqlCon);
        SqlCommand sqlCmd3 = new SqlCommand("select Count(SeriesInstanceUID) as a from Series where SeriesInstanceUID='" + value[Index_SERUID] + "'", sqlCon);
        SqlCommand sqlCmd4 = new SqlCommand("select Count(SOPInstanceUID) as a from SOPInstance where SOPInstanceUID='" + value[Index_SOPUID] + "'", sqlCon);
        SqlDataReader read1 = sqlCmd1.ExecuteReader(); read1.Read();
        SqlDataReader read2 = sqlCmd2.ExecuteReader(); read2.Read();
        SqlDataReader read3 = sqlCmd3.ExecuteReader(); read3.Read();
        SqlDataReader read4 = sqlCmd4.ExecuteReader(); read4.Read();

        //INSERT combine SQL
        if (read1["a"].ToString() == "0" || read1["a"].ToString() == null)
        {
            sqlCmd1.Cancel();
            read1.Close();
            string field = "", valuE = "";
            for (int q = 0; q < Patientcount; q++)
            {
                if (q == 0)
                {
                    field = FieldName[q].ToString();
                    if (value[q] != "null")
                        valuE = "'" + value[q].ToString() + "'";
                    else
                        valuE = value[q].ToString();
                }
                else
                {
                    field += "," + FieldName[q].ToString();
                    if (value[q] != "null")
                        valuE += "," + "'" + value[q].ToString() + "'";
                    else
                        valuE += "," + value[q].ToString();
                }
            }
            string sql = "INSERT INTO Patient(" + field + ") values(" + valuE + ")";
            sqlCmd1 = new SqlCommand(sql, sqlCon);
            read1 = sqlCmd1.ExecuteReader();
            sqlCmd1.Cancel();
            read1.Close();
        }

        if (read2["a"].ToString() == "0" || read2["a"].ToString() == null)
        {
            sqlCmd2.Cancel();
            read2.Close();
            string field = "", valuE = "";
            for (int q = Patientcount; q < Studycount; q++)
            {
                if (q == Patientcount)
                {
                    field = FieldName[q].ToString();
                    if (value[q] != "null")
                        valuE = "'" + value[q].ToString() + "'";
                    else
                        valuE = value[q].ToString();
                }
                else
                {
                    field += "," + FieldName[q].ToString();
                    if (value[q] != "null")
                        valuE += "," + "'" + value[q].ToString() + "'";
                    else
                        valuE += "," + value[q].ToString();
                }
            }
            string sql = "INSERT INTO Study(" + field + ") values(" + valuE + ")";

            sqlCmd2 = new SqlCommand(sql, sqlCon);
            read2 = sqlCmd2.ExecuteReader();
            sqlCmd2.Cancel();
            read2.Close();
        }

        if (read3["a"].ToString() == "0" || read3["a"].ToString() == null)
        {
            sqlCmd3.Cancel();
            read3.Close();
            string field = "", valuE = "";
            for (int q = Studycount; q < Seriescount; q++)
            {
                if (q == Studycount)
                {
                    field = FieldName[q].ToString();
                    if (value[q] != "null")
                        valuE = "'" + value[q].ToString() + "'";
                    else
                        valuE = value[q].ToString();
                }
                else
                {
                    field += "," + FieldName[q].ToString();
                    if (value[q] != "null")
                        valuE += "," + "'" + value[q].ToString() + "'";
                    else
                        valuE += "," + value[q].ToString();
                }
            }
            string sql = "INSERT INTO Series(" + field + ") values(" + valuE + ")";
            sqlCmd3 = new SqlCommand(sql, sqlCon);
            read3 = sqlCmd3.ExecuteReader();
            sqlCmd3.Cancel();
            read3.Close();
        }

        if (read4["a"].ToString() == "0" || read4["a"].ToString() == null)
        {
            sqlCmd4.Cancel();
            read4.Close();
            string field = "", valuE = "";
            for (int q = Seriescount; q < Imagecount; q++)
            {
                if (q == Seriescount)
                {
                    field = FieldName[q].ToString();
                    if (value[q] != "null")
                        valuE = "'" + value[q].ToString() + "'";
                    else
                        valuE = value[q].ToString();
                }
                else
                {
                    field += "," + FieldName[q].ToString();
                    if (value[q] != "null")
                        valuE += "," + "'" + value[q].ToString() + "'";
                    else
                        valuE += "," + value[q].ToString();
                }
            }
            string sql = "INSERT INTO SOPInstance(" + field + ") values(" + valuE + ")";
            sqlCmd4 = new SqlCommand(sql, sqlCon);
            read4 = sqlCmd4.ExecuteReader();
            sqlCmd4.Cancel();
            read4.Close();
        }
        else { return 500; }

        
        
        sqlCon.Close();
        sqlCon.Dispose();
        return 200;
    }
}