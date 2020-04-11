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
using System.Text;
using System.Xml;
using System.Collections.Concurrent;

namespace SmartXmlSql
{
    public class SmartSqlBuilder : IDisposable
    {
        private static ConcurrentDictionary<string, StatementItem> dicCache = new ConcurrentDictionary<string, StatementItem>();
        /// <summary>
        /// 生成SQL信息
        /// </summary>
        /// <param name="xml">文件名称</param>
        /// <param name="name">节点名称</param>
        /// <param name="obj">实体对象</param>
        /// <returns></returns>
        public SqlDef Build(string xml, string name,object obj=null)
        {
            StatementItem item = null;
            Statement statement = null;
            try
            {
                if (dicCache.TryGetValue(xml, out item))
                {
                    statement = item.GetStatement(name);
                }
                if (statement == null)
                {
                    statement = Find(xml + ".xml", name);
                    if (item == null)
                    {
                        item = new StatementItem();
                        dicCache[xml] = item;
                    }
                    item.Set(name, statement);

                }
            }
            catch
            {
               //缓存不支持不处理
            }
            statement.SqlContext = new SqlContext() { Context = obj };
            foreach (var tag in statement.Child)
            {
                //处理所有子节点
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

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="xml">文件</param>
        /// <param name="name">节点</param>
        /// <returns></returns>
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
        public void Dispose()
        {

        }
    }
}
