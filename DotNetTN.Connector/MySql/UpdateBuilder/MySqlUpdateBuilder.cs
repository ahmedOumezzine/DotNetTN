namespace DotNetTN.Connector.MySql.UpdateBuilder
{
    public class MySqlUpdateBuilder : DotNetTN.Connector.SQL.SqlBuilderProvider.UpdateBuilder
    {
        public override string SqlTemplateBatch
        {
            get
            {
                return @"UPDATE  {1} S {2}   INNER JOIN ${{0}}  SET {0} ";
            }
        }

        public override string SqlTemplateJoin
        {
            get
            {
                return @"            (
              {0}

            ) T ON {1}
                 ";
            }
        }
    }
}