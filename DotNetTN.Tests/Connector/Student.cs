using DotNetTN.Connector.SQL.Mapping;
using System;

namespace DotNetTN.Tests.Connector
{
    [Table("Studenttable")]
    public class Student
    {
        [Column(IsPrimaryKey = true, IsIdentity = true, ColumnName = "ID")]
        public int Id { get; set; }

        [Column(IsIdentity = true, IsNullable = true, Length = 22, ColumnName = "SchoolIdColumn", ColumnDescription = "blabla")]
        public int? SchoolId { get; set; }

        [Column(ColumnName = "NameColumn")]
        public string Name { get; set; }

        [Column(ColumnName = "DAteColumn")]
        public DateTime? CreateTime { get; set; }
    }
}