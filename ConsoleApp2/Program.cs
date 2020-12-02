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
            Person person = new Person() {Name = "12",  Id=34 };
            var dlg = SmartXmlSql.EmitEntityCompile.CreateParamMethod<object>(person);
            var dic=  dlg(person);
        }
    }
}
