using DotNetTN.Connector.SQL.Mapping;
using System;
using System.Collections.Generic;

namespace DotNetTN.Connector.SQL.Entities
{
    public class MappingTableList : List<MappingTable>
    {
        public void Add(string entityName, string dbTableName)
        {
            this.RemoveAll(it => it.EntityName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase));
            this.Add(new MappingTable() { EntityName = entityName, DbTableName = dbTableName });
        }

        public void Add(string entityName, string dbTableName, string dbTableShortName)
        {
            this.RemoveAll(it => it.EntityName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase));
            this.Add(new MappingTable() { EntityName = entityName, DbTableName = dbTableName, DbShortTaleName = dbTableShortName });
        }

        public new void Clear()
        {
            this.RemoveAll(it => true);
        }
    }

    public class MappingColumnList : List<MappingColumn>
    {
        public void Add(string propertyName, string dbColumnName, string entityName)
        {
            this.RemoveAll(it => it.EntityName == entityName && it.PropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            this.Add(new MappingColumn() { PropertyName = propertyName, DbColumnName = dbColumnName, EntityName = entityName });
        }

        public new void Clear()
        {
            this.RemoveAll(it => true);
        }
    }
}