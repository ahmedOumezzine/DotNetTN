using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.Connector.MySql.DeleteBuilder
{
    class MySqlDeleteBuilder: DotNetTN.Connector.SQL.SqlBuilderProvider.DeleteBuilder
    {
        public override string SqlTemplate
        {
            get
            {
                return "DELETE FROM {0}{1}";
            }
        }
        public override string WhereInTemplate
        {
            get
            {
                return "{0} IN ({1})";
            }
        }
    }
}
