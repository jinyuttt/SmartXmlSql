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
            
            //TestCls testCls = new TestCls();

            Person person = new Person() { Age = 23, Name = "12" };
          var dic=  EmitEntity.CreateParamMethod<Person>(person);
            TestCls testCls = new TestCls();
            //var lst = testCls.SearchValues(@"(<$KK>,<$FF>)");
            var lst = new List<Person>();
            lst.Add(person);
            testCls.Test(lst);
          //  SmartSqlBuilder builder = new SmartSqlBuilder();

            //var r = builder.Build("Test.xml", "test", person);
            //Console.WriteLine(r);

           
        }

       

       
    
    }
}
