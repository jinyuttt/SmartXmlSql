using Microsoft.Extensions.Caching.Memory;
using System;

namespace SmartXmlSql
{

    /// <summary>
    /// 缓存项
    /// </summary>
    public class StatementItem
    {

       

        private readonly MemoryCache Cache = null;
      
        public StatementItem()
        {
            Cache = new MemoryCache(new MemoryCacheOptions() {  ExpirationScanFrequency= TimeSpan.FromMinutes(1)});
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="statement"></param>
        public void Set(string key,Statement statement)
        {
            Cache.Set(key, statement,new MemoryCacheEntryOptions { 
             SlidingExpiration=TimeSpan.FromHours(1)} );
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public  Statement GetStatement(string key)
        {
           return Cache.Get<Statement>(key);
        }

    }
}
