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
            //设置元素过期扫描时间
            Cache = new MemoryCache(new MemoryCacheOptions() {  ExpirationScanFrequency= TimeSpan.FromMinutes(5)});
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="statement"></param>
        public void Set(string key,Statement statement)
        {
            //设置缓存并且15分钟没有使用过期
             Cache.Set(key, statement,new MemoryCacheEntryOptions { 
             SlidingExpiration=TimeSpan.FromMinutes(15)} );
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
