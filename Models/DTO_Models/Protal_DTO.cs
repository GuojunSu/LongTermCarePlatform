using LongTermCare_Xml_.Models.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace LongTermCare_Xml_.Models.DTO_Models
{
    public sealed class Protal_DTO : IMethodObject
    {
        private Dictionary<string, List<string>> Strategy;
        [DefaultValue("2.16.886.104.100661.100011")]
        public string UUID { get; set; }
        [DefaultValue(0)]
        public int User_Tree_No { get; set; }
        [DefaultValue(0)]
        public int User_Type_No { get; set; }
        [DefaultValue("")]
        public string PatientName { get; set; }
        [DefaultValue("")]
        public string PatientID { get; set; }
        [DefaultValue("")]
        public string Password { get; set; }
        [DefaultValue("")]
        public string Email { get; set; }
        [DefaultValue(0)]
        public int PatientsSexs { get; set; }
        string _PatientsSexs = "";
        [DefaultValue("")]
        public string PatientsSex
        {
            get
            {
                PatientsSexs = _PatientsSexs.Equals("M") ? 0 : 1;
                return _PatientsSexs.Equals("M") ? "0" : "1";
            }
            set { _PatientsSexs = value; }
        }
        [DefaultValue(0)]
        public int is_leaf { get; set; }
        public Protal_DTO()
        {
            Strategy = new Dictionary<string, List<string>>();
            Strategy.Add("User_Tree_No",
                    new List<string>(
                        new string[] { "UUID", "User_Type_No" }));
            Strategy.Add("portal_account",
                   new List<string>(
                       new string[] { "PatientID", "Password" }));
            Strategy.Add("AddMember",
                  new List<string>(
                      new string[] { "User_Tree_No", "User_Type_No", "PatientName", "PatientID", "Password", "Email", "PatientID", "PatientsSexs", "is_leaf" }));
        }

        public Dictionary<string, List<string>> Get_Object_Strategy()
        {
            return Strategy;
        }
    }
}
