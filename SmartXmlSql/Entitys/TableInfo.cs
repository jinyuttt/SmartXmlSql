using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartXmlSql
{

    /// <summary>
    /// 数据库表描述
    /// </summary>
    public class TableInfo
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 列名称
        /// </summary>
        public List<ColumnInfo> ColumnInfos { get; set; }

        public TableInfo()
        {
            if (this.ColumnInfos == null)
            {
                this.ColumnInfos = new List<ColumnInfo>();
            }
        }
    }
}
