using System;

namespace DotNetTN.Connector.SQL.Entities
{
    public class DbResult<T>
    {
        public bool IsSuccess { get; set; }
        public Exception ErrorException { get; set; }
        public string ErrorMessage { get; set; }
        public T Data { get; set; }
    }
}