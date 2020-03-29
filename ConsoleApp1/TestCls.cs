using SmartXmlSql;
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

       public  List<string> SearchValues(string sql)
        {
            List<string> sqlPi = new List<string>();
            Regex regex = new Regex(@"<([^<>]*)>");//小括号
            MatchCollection matchs = regex.Matches(sql);
            foreach (Match match in matchs)
            {
                sqlPi.Add(match.Groups[0].Value);
            }
            return sqlPi;
        }
    }
}
