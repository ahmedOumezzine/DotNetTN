using DotNetTN.Connector.SQL.SqlBuilderProvider;

namespace DotNetTN.Connector.MySql.Builder
{
    public class MySqlBuilder : SqlBuilderProvider
    {
        public override string SqlTranslationLeft { get { return "`"; } }
        public override string SqlTranslationRight { get { return "`"; } }
    }
}