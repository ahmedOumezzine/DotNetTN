using DotNetTN.Connector.SQL;
using DotNetTN.Connector.SQL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.Tests.Connector
{
    public class Base
    {
        public static string ConnectionString = "server=localhost;Database=Dotnetdb;Uid=root;Pwd=";

        public static SqlClient GetInstance()
        {
            SqlClient db = new SqlClient(new Config() { ConnectionString = ConnectionString, DbType = DbType.MySql, IsAutoCloseConnection = true });
           
            return db;
        }
    }
}
