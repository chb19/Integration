using System;

namespace RedisDataBase
{
    public class RedisBase
    {
        public static string ReadData(string key)
        {
            var cache = RedisConnectorHelper.Connection.GetDatabase();
                return cache.StringGet(key);
        }

        public static void SaveBigData(string key, string value)
        {
            var cache = RedisConnectorHelper.Connection.GetDatabase();

            cache.StringSet(key, value);
        }
    }
}
