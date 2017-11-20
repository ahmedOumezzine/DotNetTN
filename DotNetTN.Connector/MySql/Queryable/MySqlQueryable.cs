using DotNetTN.Connector.SQL.SqlBuilderProvider;

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

    public class MySqlQueryable<T, T2> : QueryableProvider<T, T2>
    {
    }
}