using SmartXmlSql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
   public class TestCls
    {
        public void TestList<T>(T oj)
        {
           var slq=  SqlGenerator.Instance.Builder(oj);
        }
        public void TestBatch<T>(T oj)
        {
            var slq = SqlGenerator.Instance.Builder(oj);
        }
        public void TestArray<T>(T oj)
        {
            var slq = SqlGenerator.Instance.Builder(oj);
        }

        public void TestEntity<T>(T oj)
        {
            var slq = SqlGenerator.Instance.Builder(oj);
        }

        public void TestPartEntity<T>(T oj)
        {
            var slq = SqlGenerator.Instance.Builder(oj);
        }

        public void QueryStu<T>(T oj)
        {
            var slq = SqlGenerator.Instance.Builder(oj);
        }

        public void UpdateStu<T>(T oj)
        {
            var slq = SqlGenerator.Instance.Builder(oj);
        }
        public void QueryStuById<T>(T id)
        {
            var slq = SqlGenerator.Instance.Builder(id);
        }

        public void QueryStuByName<T>(T name)
        {
            var slq = SqlGenerator.Instance.Builder(name);
        }
    }
}
