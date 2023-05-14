using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartXmlSql.Entitys
{
    public class ColumnDescription
    {

        public string TableName { get; set; }
        public string ColumnName { get; set; } = string.Empty;

        /// <summary>
        /// 列名称
        /// </summary>
        public List<ColumnInfo> ColumnInfos { get; set; }

        public ColumnDescription()
        {
            if (this.ColumnInfos == null)
            {
                this.ColumnInfos = new List<ColumnInfo>();
            }
        }
    }
}
