using SmartXmlSql;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Person person = new Person() { Age = 23, Name = "12" };
            TestCls testCls = new TestCls();
            testCls.Test(person);
            SmartSqlBuilder builder = new SmartSqlBuilder();
           
           var r= builder.Build("Test.xml", "test",person);
            Console.WriteLine(r);
        }

       

       
    
    }
}
