﻿using DotNetTN.Connector.SQL.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;

namespace DotNetTN.Connector.SQL.Common
{
    public class RewritableMethods : IRewritableMethods
    {
        public SqlClient CopyContext(SqlClient context, bool isCopyEvents = false)
        {
            throw new NotImplementedException();
        }

        public List<T> DataReaderToDynamicList<T>(IDataReader reader)
        {
            throw new NotImplementedException();
        }

        public ExpandoObject DataReaderToExpandoObject(IDataReader reader)
        {
            throw new NotImplementedException();
        }

        public List<ExpandoObject> DataReaderToExpandoObjectList(IDataReader reader)
        {
            throw new NotImplementedException();
        }

        public dynamic DataTableToDynamic(DataTable table)
        {
            throw new NotImplementedException();
        }

        public T DeserializeObject<T>(string value)
        {
            throw new NotImplementedException();
        }

        public ICacheManager<T> GetCacheInstance<T>()
        {
            return CacheManager<T>.GetInstance();
        }

        public void RemoveCache<T>(string key)
        {
            throw new NotImplementedException();
        }

        public void RemoveCacheAll()
        {
            throw new NotImplementedException();
        }

        public void RemoveCacheAll<T>()
        {
            throw new NotImplementedException();
        }

        public string SerializeObject(object value)
        {
            throw new NotImplementedException();
        }

        public T TranslateCopy<T>(T sourceObject)
        {
            throw new NotImplementedException();
        }
    }
}