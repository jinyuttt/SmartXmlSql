﻿using SmartXmlSql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
   public class TestCls
    {
        public void Test<T>(T oj)
        {
            SqlGenerator generator = new SqlGenerator();
          var content=  generator.builder(oj);

        }

      
    }
}
