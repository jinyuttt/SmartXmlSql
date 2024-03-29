﻿#region   文件版本注释
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
using System.IO;
using SmartXmlSql.statements;
using SmartXmlSql.Cache;

namespace SmartXmlSql
{

    /// <summary>
    /// 输出SQL和参数
    /// 处理XML
    /// </summary>
    internal class SmartSqlBuilder : IDisposable
    {
        /// <summary>
        /// xml文件内容
        /// </summary>
        private static ConcurrentDictionary<string, StatementItem> dicCache = new ConcurrentDictionary<string, StatementItem>();
        
        /// <summary>
        /// 生成SQL信息
        /// </summary>
        /// <param name="xmlfile">文件名称</param>
        /// <param name="node">节点名称</param>
        /// <param name="obj">实体对象</param>
        /// <returns></returns>
        public SqlDef Build(string xmlfile, string node, object obj = null)
        {
            StatementItem item = null;
            Statement statement = null;
            try
            {
                if (dicCache.TryGetValue(xmlfile, out item))
                {
                    statement = item.GetStatement(node);
                }
                if (statement == null)
                {
                    string file = Path.Combine(SmartXmlSqlCfg.XmlDir, xmlfile) + ".xml";
                    statement = Find(file, node);
                    if (item == null)
                    {
                        item = new StatementItem();
                        dicCache[xmlfile] = item;
                    }
                    item.Set(node, statement);

                }
            }
            catch(StatementException ex)
            {
                //缓存不支持不处理
                throw ex;
            }

            //BuildSql使用
            statement.SqlContext = new SqlContext() { Context = obj };
            foreach (var tag in statement.Child)
            {
                //处理所有子节点
                tag.BuildSql();
            }
            StringBuilder builder = new StringBuilder();
            foreach (var tag in statement.Child)
            {
               //所以节点组合
                builder.Append(tag.GetSql());
            }

            SqlDef def = new SqlDef() { Acess = statement.Acess, DB = statement.DB, Key = statement.Key, SQL = builder.ToString() };
            return def;
        }

        /// <summary>
        /// 获取配置信息，生成语句
        /// </summary>
        /// <param name="xml">文件</param>
        /// <param name="name">节点</param>
        /// <returns></returns>
        private Statement Find(string xml, string name)
        {
            if(!File.Exists(xml))
            {
                throw new StatementException("未知", xml, "没有找到该xml文件");
            }
            XmlDocument document = new XmlDocument();
            document.Load(xml);
            string targetParm = string.Format("SqlMapper/Statement[@Id='{0}']", name);//生成目标获取节点的参数
            var targetNode = document.SelectSingleNode(targetParm);
            if (targetNode == null)
            {
                throw new StatementException("未知", name, "没有找到该节点");
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
            dicCache.Clear();
        }
    }
}
