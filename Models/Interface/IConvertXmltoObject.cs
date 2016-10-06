using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LongTermCare_Xml_.Models.Interface
{
    public interface IConvertXmltoObject
    {
       T ConvertValue<T>(object A);
    }
}
