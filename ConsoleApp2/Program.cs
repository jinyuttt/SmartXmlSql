using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Person person = new Person() {Name = "12" };
            var dic = SmartXmlSql.EmitEntity.CreateParamMethod<Person>(person);
        }
    }
}
