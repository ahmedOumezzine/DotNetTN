﻿using DotNetTN.Connector.SQL.Mapping;
using System;

namespace DotNetTN.Tests.Connector
{
    [Table("STudent")]
    public class Student
    {
        [Column(IsPrimaryKey = true, IsIdentity = true, ColumnName = "ID")]
        public int Id { get; set; }

        public int? SchoolId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
    }
}