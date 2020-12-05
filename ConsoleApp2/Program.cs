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

            string str = typeof(string).Name.ToLower();
            str = typeof(DateTime).Name.ToLower();
            Person person = new Person() {Name = "12",  Id=34 };
            var dlg =SmartXmlSql.EmitObjectCompile.CreateParamMethod(person);
            var dic=  dlg(person);
        }
    }
}
