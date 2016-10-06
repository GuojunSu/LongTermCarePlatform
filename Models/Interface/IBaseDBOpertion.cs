using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LongTermCare_Xml_.Models.Interface
{
    interface IBaseDBOpertion<in InEntity>
    {
        object BaseInsert();
        void BaseSearch(InEntity InputObject);
        object BaseDelete();
        object BaseUpdate();
    }
}
