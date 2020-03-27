using SmartXmlSql;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
   public class TestCls
    {
        public void Test<T>(T oj)
        {
            SqlGenerator generator = new SqlGenerator();
            generator.builder(oj);
        }
    }
}
