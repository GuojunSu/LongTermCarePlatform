using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LongTermCare_Xml_.Models.Xml_Operation
{
    public class State
    {
        //private int RowCount = 0;
        private int CurrentCount = 0;
        private int LayerCount = 0;
        private string PrimaryKey = "";
        private string ParentPrimaryKey = "";
        public State(int CurrentCount, int LayerCount, string PrimaryKey, string ParentPrimaryKey)
        {
            this.CurrentCount = CurrentCount;
            //this.RowCount = RowCount;
            this.LayerCount = LayerCount;
            this.PrimaryKey = PrimaryKey;
            this.ParentPrimaryKey = ParentPrimaryKey;
        }

        //public int GetRowCount()
        //{
        //    return RowCount;
        //}

        public int GetCurrentCount()
        {
            return CurrentCount;
        }

        public int GetLayerCount()
        {
            return LayerCount;
        }

        public string GetPrimaryKey()
        {
            return PrimaryKey;
        }

        public string GetParentPrimaryKey()
        {
            return ParentPrimaryKey;
        }
    }
}