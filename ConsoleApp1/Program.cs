using SmartXmlSql;
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
            long tskid = 3;
            Test(tskid);
            //TestCls testCls = new TestCls();

            Person person = new Person() { Age = 23, Name = "12" };
          var dic=  EmitEntityCompile.CreateParamMethod<Person>(person);
            TestCls testCls = new TestCls();
            //var lst = testCls.SearchValues(@"(<$KK>,<$FF>)");
            var lst = new List<Person>();
            lst.Add(person);
            testCls.Test(lst);
          //  SmartSqlBuilder builder = new SmartSqlBuilder();

            //var r = builder.Build("Test.xml", "test", person);
            //Console.WriteLine(r);

           
        }

        private static void Test( params object[] args)
        {
            if(args.Length == 1 && args[0].GetType().IsClass && args[0].GetType() != typeof(string))
            {
                Console.WriteLine("1");
            }
            var r = args[0].GetType();
        }
       

       
    
    }
}
