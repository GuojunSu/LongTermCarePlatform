using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LongTermCare_Xml_.Models.Interface
{
    public interface IMethodObject
    {
        Dictionary<string, List<string>> Get_Object_Strategy();
       // object[] GetMethodObjects(Object InputObject,string Key);
    }
}
