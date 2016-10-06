
using LongTermCare_Xml_.Models.DTO_Models;
using LongTermCare_Xml_.Models.Interface;
using LongTermCare_Xml_.WebReference;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LongTermCare_Xml_.Models.Protal_Operation
{
    public static class ProtalRegistration
    {
        private static notech _Protal_Service = new notech();
        public static object Check<T, IN>(T _ProtalDTO, string Attri_Name, Func<IN> Method)
        {
            if (Attri_Name.Equals(""))
                return Method();
            _ProtalDTO.GetType().GetProperty(Attri_Name).SetValue(_ProtalDTO, Method(), null);
            return _ProtalDTO;
        }

        public static T Get_Portal_Type<T>(this T valid, string type)
        {
            return (T)Check(valid, "User_Type_No", () => _Protal_Service.search_portal_type(type));
        }

        public static T Get_Tree_No<T>(this T valid)
        {
            MethodObject.SetClass(valid);
            MethodInfo _Method = _Protal_Service.GetType().GetMethod("search_tree_no");
            return (T)Check(valid, "User_Tree_No", () => _Method.Invoke(_Protal_Service, valid.GetMethodObjects("User_Tree_No")));
        }

        public static OUT IsRegistration<T, OUT>(this T valid, OUT DefalutResult)
        {
            MethodObject.SetClass(valid);
            MethodInfo _Method = _Protal_Service.GetType().GetMethod("search_portal_account");
            return DefalutResult = (OUT)Check(valid, "", () => _Method.Invoke(_Protal_Service, valid.GetMethodObjects("portal_account")));
        }

        public static OUT AddMember<T, OUT>(this T valid, OUT DefalutResult)
        {
            object[] temps = valid.GetMethodObjects("AddMember");
            MethodObject.SetClass(valid);
            MethodInfo _Method = _Protal_Service.GetType().GetMethod("import");
            return DefalutResult = (OUT)Check(valid, "", () => _Method.Invoke(_Protal_Service, valid.GetMethodObjects("AddMember")));
        }
    }
}