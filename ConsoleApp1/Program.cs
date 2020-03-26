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
            SmartSqlBuilder builder = new SmartSqlBuilder();
            Person person = new Person() { Age = 23, Name = "" };
           var r= builder.Build("Test.xml", "test",person);
            Console.WriteLine(r);
        }

       public bool CreateBool(object  p)
        {
            Person person = p as Person;
          return  string.IsNullOrEmpty(person.Name);
        }

       
    
    }
}
