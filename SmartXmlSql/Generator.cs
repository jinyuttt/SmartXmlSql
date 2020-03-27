using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace SmartXmlSql
{
  public  class SqlGenerator
    {
        SmartSqlBuilder smartSql = new SmartSqlBuilder();
        public BuilderContent builder(params object[] args)
        {
            StackTrace stackTrace = new StackTrace();
            var mth = stackTrace.GetFrame(1).GetMethod();
            ParameterInfo[] parameters = mth.GetParameters();
            string className = mth.ReflectedType.Name;
            string name = mth.Name;
            string sql = "";
            if (args.Length == 0)
            {
                sql = smartSql.Build(className, name);
            }
            else
            {
                sql = smartSql.Build(className, name, args[0]);
            }
            //
            Dictionary<string, SqlValue> dic = new Dictionary<string, SqlValue>();
            if(args.Length==1&&args[0].GetType().IsClass)
            {
                //遍历属性
            }
            else
            {
                //遍历参数替换
            }
            BuilderContent content = new BuilderContent() { Sql = sql, SqlParam=dic };
            return content;
        }
    }
}
