using System;
using System.Collections.Generic;
using System.Text;

namespace SmartXmlSql.Cache
{

    /// <summary>
    /// 参数信息
    /// </summary>
    public class SqlKV
    {
        /// <summary>
        /// 关键字：方法参数
        /// </summary>
        public string Key { get; set; }

        public SqlValue Value { get; set; }
    }
}
