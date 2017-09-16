namespace DotNetTN.Connector.SQL.Entities
{
    public class JoinQueryInfo
    {
        public JoinType JoinType { get; set; }
        public string TableName { get; set; }
        public string ShortName { get; set; }
        public int JoinIndex { get; set; }
        public string JoinWhere { get; set; }
    }
}