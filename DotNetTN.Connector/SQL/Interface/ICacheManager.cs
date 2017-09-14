using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.Connector.SQL.Interface
{
    public interface ICacheManager<V>
    {
        V this[string key] { get; }
        void Add(string key, V value);
        void Add(string key, V value, int cacheDurationInSeconds);
        bool ContainsKey(string key);
        V Get(string key);
        IEnumerable<string> GetAllKey();
        void Remove(string key);
        V Func(string cacheKey, Func<ICacheManager<V>, string, V> successAction, Func<ICacheManager<V>, string, V> errorAction);
        void Action(string cacheKey, Action<ICacheManager<V>, string> successAction, Func<ICacheManager<V>, string, V> errorAction);
    }
}