using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SmartXmlSql
{
    public  class SqlGenerator
    {
        SmartSqlBuilder smartSql = new SmartSqlBuilder();
        public BuilderContent builder(params object[] args)
        {
            StackTrace stackTrace = new StackTrace();
            var mth = stackTrace.GetFrame(1).GetMethod();
          
            string className = mth.ReflectedType.Name;
            string name = mth.Name;
            SqlDef sql =null;
            if (args.Length == 0)
            {
                sql = smartSql.Build(className, name);
            }
            else
            {
                sql = smartSql.Build(className, name, args[0]);
            }
            //SQL参数提取
           
            
            Dictionary<string, SqlValue> dic = new Dictionary<string, SqlValue>();
            if (args.Length==1&&args[0].GetType().IsClass&& args[0].GetType()!=typeof(string))
            {
                //遍历属性
                object arg = args[0];
                var properties=  arg.GetType().GetProperties();
                foreach(var p in properties)
                {
                    SqlValue value = new SqlValue() { DataType = p.PropertyType.FullName, Value = p.GetValue(arg).ToString() };
                    dic["@"+p.Name.ToLower()] = value;
                }
            }
            else
            {
                //遍历参数替换
                ParameterInfo[] parameters = mth.GetParameters();
                for(int i=0;i<parameters.Length;i++)
                {
                    var p = parameters[i];
                    SqlValue value = new SqlValue() { DataType = p.ParameterType.FullName, Value = args[i].ToString() };
                    dic["@" + p.Name.ToLower()] = value;
                }
            }
            //
            var sqlPi = SearchParam(sql.SQL);
            Dictionary<string, SqlValue> dicParam = new Dictionary<string, SqlValue>();
            if(sql.Key=="Entity"&&sqlPi.Count==1)
            {
                StringBuilder p = new StringBuilder();
                StringBuilder v = new StringBuilder();
                foreach(var kv in dic)
                {
                    p.Append(kv.Key.Substring(1) + ",");
                    v.Append(kv.Key + ",");

                }
                p.Remove(p.Length - 1, 1);
                v.Remove(v.Length - 1, 1);
                //
                string tmp = sqlPi[0].Substring(1);
                sql.SQL = sql.SQL.Replace("$" + tmp, p.ToString());
                sql.SQL = sql.SQL.Replace("@" + tmp, v.ToString());

            }
            else if(sql.Key== "Array" && sqlPi.Count == 1)
            {
                StringBuilder p = new StringBuilder();
                bool isStr = false;
                if(args[0].GetType()==typeof(string))
                {
                    isStr = true;
                }
                foreach(var arg in args)
                {
                    if (isStr)
                    {
                        p.Append("'" + arg + "',");
                    }
                    else
                    {
                        p.Append(arg+",");
                    }
                }
                p.Remove(p.Length-1, 1);
                sql.SQL = p.ToString();
            }
            foreach (var p in sqlPi)
            {
                SqlValue value = null;
                if (dic.TryGetValue(p.ToLower(),out value))
                {
                    dicParam[p] = value;
                }
            }
            BuilderContent content = new BuilderContent() { Sql = sql.SQL, SqlParam= dicParam };
            return content;
        }
         

        private List<string>  SearchParam(string sql)
        {
            List<string> sqlPi = new List<string>();
            Regex regex = new Regex(@"(?<!@)[^\w$#@]@(?!@)[\w$#@]+");
            MatchCollection matchs = regex.Matches(sql);
            foreach (Match match in matchs)
            {
                sqlPi.Add(match.Groups[0].Value.Substring(match.Groups[0].Value.IndexOf("@")));
            }
            return sqlPi;
        }


        private List<string> SearchReplace(string sql)
        {
            List<string> sqlPi = new List<string>();
            Regex regex = new Regex(@"(?<!)[^\w$#@]@(?!@)[\w$#@]+");
            MatchCollection matchs = regex.Matches(sql);
            foreach (Match match in matchs)
            {
                sqlPi.Add(match.Groups[0].Value.Substring(match.Groups[0].Value.IndexOf("@")));
            }
            return sqlPi;
        }
    }
}
