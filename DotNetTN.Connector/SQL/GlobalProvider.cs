using DotNetTN.Connector.MySql;
using DotNetTN.Connector.MySql.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.Connector.SQL
{
    internal class GlobalProvider
    {
        private static bool IsTryMySql = false;


        public static void TryMySqlData()
        {
            if (!IsTryMySql)
            {
                try
                {
                    MySqlProvider db = new MySqlProvider();
                    var conn = db.GetAdapter();
                    IsTryMySql = true;
                }
                catch
                {
                    throw new Exception("You need to refer to MySql.Data.dl");
                }
            }
        }
    }
}
