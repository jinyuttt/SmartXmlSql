using System;
using System.Collections.Generic;
using System.Text;

namespace SmartXmlSql
{
   public class EmitDemo
    {
        public Dictionary<string, SqlValue> GetP(Person person)
        {
            Dictionary<string, SqlValue> dic = new Dictionary<string, SqlValue>();
          
            dic["@name"]= new SqlValue() { DataType = "string", Value =Convert.ToString(person.Name) };
            return dic;
        }
        public Dictionary<string, SqlValue> GetP(int id,string name)
        {
            Dictionary<string, SqlValue> dic = new Dictionary<string, SqlValue>();
            dic["@id"] = new SqlValue() { DataType = "int32", Value =id.ToString() };
            dic["@name"] = new SqlValue() { DataType = "string", Value = name };
            return dic;
        }
    }

    public class Person
    {
       

        public string Name { get; set; }
    }
}
