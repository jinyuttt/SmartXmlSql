#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*项目名称 ：SmartXmlSql
*项目描述 ：
*命名空间 ：SmartXmlSql
*文件名称 ：SmartSqlBuilder.cs
* 功能描述 ：SmartSqlBuilder
* 创建时间 ：2020
*版本号   :   2020|V1.0.0.0 
---------------------------------------------------------------------
* Copyright @ jinyu 2020. All rights reserved.
---------------------------------------------------------------------

***********************************************************************/
#endregion



using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SmartXmlSql
{
    public class SmartSqlBuilder : IDisposable
    {
        public void Dispose()
        {
           
        }
        public SqlDef Build(string xml, string name,object obj=null)
        {
            var statement = Find(xml+".xml", name);
            statement.SqlContext = new SqlContext() { Context = obj };
            foreach (var tag in statement.Child)
            {
                tag.BuildSql();
            }
            StringBuilder builder = new StringBuilder();
            foreach (var tag in statement.Child)
            {
                builder.Append(tag.GetSql());
            }
            SqlDef def = new SqlDef() { Acess = statement.Acess, DB = statement.DB, Key = statement.Key, SQL = builder.ToString() };
            return def;
        }

        private Statement Find(string xml,string name)
        {
            XmlDocument document = new XmlDocument();
            document.Load(xml);
            string targetParm = string.Format("SqlMapper/Statement[@Id='{0}']", name);//生成目标获取节点的参数
           var targetNode= document.SelectSingleNode(targetParm);
            if (targetNode == null)
            {
                Console.WriteLine("can not find");
                return null;
            }
            else
            {
                Analysis analysis = new Analysis();
                Statement statement = new Statement();
                analysis.AnalysisStatement(targetNode, statement);
                return statement;
            }
          
        }
    }
}
