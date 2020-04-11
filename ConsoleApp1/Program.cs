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
            TestCls testCls = new TestCls();
            //var lst = testCls.SearchValues(@"(<$KK>,<$FF>)");
            testCls.Test(new List<Person>() {  person});
            SmartSqlBuilder builder = new SmartSqlBuilder();

            //var r = builder.Build("Test.xml", "test", person);
            //Console.WriteLine(r);

           
        }

       

       
    
    }
}
