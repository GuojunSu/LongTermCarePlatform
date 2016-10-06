using LongTermCare_Xml_.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace LongTermCare_Xml_.Models.DTO_Models
{
    public static class MethodObject
    {
        private static IMethodObject _InputObject { get; set; }

        public static void SetClass<IMethod>(IMethod InputObject)
        {
            _InputObject = (IMethodObject)InputObject;
        }

        public static object[] GetMethodObjects<T>(this T Object, string Key)
        {
            object[] TempObjects = null;
            int count = 0;
            Dictionary<string, List<string>> TEMP = _InputObject.Get_Object_Strategy();
            List<string> Parameters = TEMP.Where(ob => Key.Contains(ob.Key)).Select(ob => ob.Value).FirstOrDefault();
            TempObjects = new object[Parameters.Count];
            foreach (string Patam in Parameters)
                TempObjects.SetValue(Object.GetType().GetProperties().Where(ob => ob.Name == Patam).Select(ob => ob.GetValue(Object)).FirstOrDefault(), count++);
            return TempObjects;
        }
    }
}