using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LongTermCare_Xml_.Models.SQL_Operation
{
    public class BaseSQL :IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public virtual object[] GetSQLObjects<T>(T SQL_DTO)
        {
            object[] TempObjects = null;
            int count = 0, StrategyCount;
            //取得物件策略
            try
            {
                Dictionary<string, List<string>> TEMP = (Dictionary<string, List<string>>)SQL_DTO.GetType().GetMethod("Get_Object_Strategy").Invoke(SQL_DTO, null);
                List<string> Strategy = TEMP.First().Value;
                StrategyCount = Strategy.Count;
                TempObjects = new object[StrategyCount];
                foreach (PropertyInfo Prop in SQL_DTO.GetType().GetProperties())
                {
                    //確認物件策略取得Condition;table
                    if (Strategy.Any(valueName => Prop.Name.Equals(valueName)) && StrategyCount != 0)
                    {
                        TempObjects.SetValue(Prop, count++);
                        StrategyCount--;
                    }
                    else
                        break;
                }
            }
            catch (Exception err)
            {
                err.ToString();
            }
            return TempObjects;
        }

        public virtual PropertyInfo GetSQLObject<T>(T SQL_DTO)
        {
            //取得物件策略
            try
            {
                Dictionary<string, List<string>> TEMP = (Dictionary<string, List<string>>)SQL_DTO.GetType().GetMethod("Get_Object_Strategy").Invoke(SQL_DTO, null);
                List<string> Strategy = TEMP.First().Value;
                foreach (PropertyInfo Prop in SQL_DTO.GetType().GetProperties())
                {
                    //確認物件策略取得Condition;table
                    if (Strategy.Any(valueName => Prop.Name.Equals(valueName)))
                        return Prop;
                    else
                        break;
                }
            }
            catch (Exception err)
            {
                err.ToString();
            }
            return null;
        }

    }
}