using System;
using System.Collections.Generic;
using System.Text;

namespace SmartXmlSql
{
   public class BuilderContent
    {
        public string Sql { get; set; }

        public Dictionary<string, SqlValue> SqlParam { get; set; }
    }

    public class SqlValue
    {
        public string Value { get; set; }

        public string DataType { get; set; }
    }
}
