using System.Reflection;

namespace DotNetTN.Connector.SQL.Entities
{
    public class EntityColumnInfo
    {
        public PropertyInfo PropertyInfo { get; set; }
        public string PropertyName { get; set; }
        public string DbColumnName { get; set; }
        public string OldDbColumnName { get; set; }
        public int Length { get; set; }
        public string ColumnDescription { get; set; }
        public string DefaultValue { get; set; }
        public bool IsNullable { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsPrimarykey { get; set; }
        public string EntityName { get; set; }
        public string DbTableName { get; set; }
        public bool IsIgnore { get; set; }
        public string DataType { get; set; }
        public int DecimalDigits { get; set; }
    }
}