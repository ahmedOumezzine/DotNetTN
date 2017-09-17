using DotNetTN.Connector.SQL.SqlBuilderProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.Connector.MySql.Queryable
{
    public class MySqlQueryable<T> : QueryableProvider<T>
    {
        public override SQL.Interface.IQueryable<T> With(string withString)
        {
            return this;
        }

        public override SQL.Interface.IQueryable<T> PartitionBy(string groupFileds)
        {
            this.GroupBy(groupFileds);
            return this;
        }
    }
}