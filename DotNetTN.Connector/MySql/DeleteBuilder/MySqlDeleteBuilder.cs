namespace DotNetTN.Connector.MySql.DeleteBuilder
{
    internal class MySqlDeleteBuilder : DotNetTN.Connector.SQL.SqlBuilderProvider.DeleteBuilder
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