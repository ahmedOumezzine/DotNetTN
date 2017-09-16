using System;
using System.Collections.Generic;

namespace DotNetTN.Connector.SQL.Entities
{
    public class EntityInfo
    {
        private string _DbTableName;
        public string EntityName { get; set; }
        public string DbTableName { get { return _DbTableName == null ? EntityName : _DbTableName; } set { _DbTableName = value; } }
        public Type Type { get; set; }
        public List<EntityColumnInfo> Columns { get; set; }
    }
}