using System;
using System.Collections.Generic;
using System.Text;

namespace SmartXmlSql
{
    /// <summary>
    /// SQL内容
    /// </summary>
   public class BuilderContent
    {
        /// <summary>
        /// SQL语句
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// SQL参数
        /// </summary>
        public Dictionary<string, SqlValue> SqlParam { get; set; }
    }

    /// <summary>
    /// SQL参数信息
    /// </summary>
    public class SqlValue
    {
        public string Value { get; set; }

        public string DataType { get; set; }
    }
}
