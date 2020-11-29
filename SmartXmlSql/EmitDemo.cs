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
            dic["@id"] = new SqlValue() { DataType = person.Id.GetType().Name, Value = person.Id.ToString() };
            dic["@name"]= new SqlValue() { DataType = person.Name.GetType().Name, Value = person.Name };
            return dic;
        }
        public Dictionary<string, SqlValue> GetP(int id,string name)
        {
            Dictionary<string, SqlValue> dic = new Dictionary<string, SqlValue>();
            dic["@id"] = new SqlValue() { DataType = id.GetType().Name, Value =id.ToString() };
            dic["@name"] = new SqlValue() { DataType = name.GetType().Name, Value = name };
            return dic;
        }
    }

    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
