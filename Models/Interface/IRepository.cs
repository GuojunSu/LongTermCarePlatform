using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LongTermCare_Xml_.Models.Interface
{
    interface IRepository<TEntity>
    {
        string Download_List();
        string Get_ListVersion();
        string Update_List();
    }
}
