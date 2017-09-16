namespace DotNetTN.Connector.SQL.Entities
{
    public class Config
    {
        /// <summary>
        ///DbType.SqlServer Or Other
        /// </summary>
        public DbType DbType { get; set; }

        /// <summary>
        ///Database Connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// true does not need to close the connection
        /// </summary>
        public bool IsAutoCloseConnection { get; set; }

        /// <summary>
        /// Default SystemTable,If you do not have system table permissions, use attribute
        /// </summary>
        public InitKeyType InitKeyType = InitKeyType.SystemTable;
    }
}