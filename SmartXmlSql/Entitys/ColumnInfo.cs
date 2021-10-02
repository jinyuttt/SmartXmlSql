using System;

namespace SmartXmlSql
{
    public class ColumnInfo
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public Type ColumnType { get; set; }

        /// <summary>
        /// 列名说明
        /// </summary>
        public string Description { get; set; }
    }
}