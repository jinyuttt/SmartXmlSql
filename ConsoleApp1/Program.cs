using SmartXmlSql;
using SmartXmlSql.Entitys;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            TestCls();
            //long tskid = 3;
             Dictionary<string,SqlValue> dicSql = new Dictionary<string,SqlValue>();
             Dictionary<string,object> obj=  dicSql.ToObjectParam();
            //Test(tskid);
            //for (int i = 0; i < 1000; i++)
            //{
            //    Person person = new Person() { Age = 23, Name = "12", Address = "ddd" };

            //    TestCls testCls = new TestCls();

            //    var lst = new List<Person>();
            //    lst.Add(person);
            //    person = new Person() { Age = 34, Name = "jin", Address = "ddd", Id = 44 };
            //    lst.Add(person);
            //    testCls.TestList(lst);
            //    testCls.TestBatch(lst);
            //    string[] user = new string[] { "tom", "jin", "yu" };
            //    testCls.TestArray(user);
            //    testCls.TestEntity(person);
            //    testCls.TestPartEntity(person);
            //    testCls.QueryStu(person);
            //    testCls.UpdateStu(person);
            //    testCls.QueryStuById(34);

            //    testCls.QueryStuByName("jinyu");
            //}
        }

        private static void Test(params object[] args)
        {
            if (args.Length == 1 && args[0].GetType().IsClass && args[0].GetType() != typeof(string))
            {
                Console.WriteLine("1");
            }
            var r = args[0].GetType();
        }


        private static void TestCls()
        {
            var ss = new TableInfo() { TableName = "stud" };
            ss.ColumnInfos.Add(new ColumnInfo() { ColumnName = "id", ColumnType = typeof(int), Description = "编号" });
            ss.ColumnInfos.Add(new ColumnInfo() { ColumnName = "name", ColumnType = typeof(string), Description = "名称" });

            DBCodeEntity.CreateCodeFile(ss, "ss.cs", "mytest", "myserver");

            //myserver.mytest mytest = new myserver.mytest();
            //mytest.name = "ss";
        }

    }
}
