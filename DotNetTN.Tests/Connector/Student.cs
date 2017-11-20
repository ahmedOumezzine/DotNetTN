using DotNetTN.Connector.SQL.Mapping;
using System;

namespace DotNetTN.Tests.Connector
{
    [Table("School")]
    public class School
    {
        [Column(IsPrimaryKey = true, IsIdentity = true, ColumnName = "resumeID")]
        public int Id { get; set; }

        [Column(ColumnName = "fullName")]
        public string Name { get; set; }
    }

    [Table("resume")]
    public class resumeItem
    {
        [Column(IsPrimaryKey = true, IsIdentity = true, ColumnName = "resumeID")]
        public int ID { get; set; }

        [Column(ColumnName = "fullName")]
        public string Name { get; set; }

        [Column(ColumnName = "fullPrenom")]
        public string Prenom { get; set; }

        [Column(ColumnName = "DatedeNaissance")]
        public DateTime? Date { get; set; }

        [Column(ColumnName = "Originaire")]
        public string Originaire { get; set; }

        [Column(ColumnName = "EtatCivil")]
        public string EtatCivil { get; set; }
    }

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