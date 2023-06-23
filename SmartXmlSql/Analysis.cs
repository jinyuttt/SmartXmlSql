#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*项目名称 ：SmartXmlSql
*项目描述 ：
*命名空间 ：SmartXmlSql
*文件名称 ：Analysis.cs
* 功能描述 ：Analysis
* 创建时间 ：2020
*版本号   :   2020|V1.0.0.0 
---------------------------------------------------------------------
* Copyright @ jinyu 2020. All rights reserved.
---------------------------------------------------------------------

***********************************************************************/
#endregion



using System;
using System.Reflection;
using System.Xml;
using System.Linq;
using SmartXmlSql.statements;

namespace SmartXmlSql
{

    /// <summary>
    /// 解析XML
    /// </summary>
    public class Analysis
    {
        /// <summary>
        /// xml获取Statement
        /// </summary>
        /// <param name="node"></param>
        /// <param name="statement"></param>
        public void AnalysisStatement(XmlNode node, Statement statement)
        {
            XmlElement element = (XmlElement)node;
            statement.Key = element.GetAttribute("Key");
            statement.DB = element.GetAttribute("DB");
           
            string str=element.GetAttribute("Acess");
            if (!string.IsNullOrEmpty(str.Trim()))
            {
                AcessType acess;
                if (Enum.TryParse<AcessType>(str, out acess))
                {
                    statement.Acess = acess;
                }
            }

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Text)
                {
                    SqlText sql = new SqlText
                    {
                        Sql = child.InnerText,
                        Statement = statement
                    };
                    statement.Tags.Add(sql);
                    statement.Child.Add(sql);
                    continue;
                }
                AnalysisNode(child, null, statement);
            }

        }
       private void AnalysisNode(XmlNode node, ITag condtion, Statement statement)
        {
            if (node.NodeType == XmlNodeType.Text)
            {
                SqlText sql = new SqlText
                {
                    Sql = node.InnerText,
                    Statement = statement,
                    Parent = condtion
                };
                statement.Tags.Add(sql);
                return;
            }
            //通过名称映射
            var type = Assembly.GetExecutingAssembly().DefinedTypes.Where(X => X.Name == node.Name && typeof(ITag).IsAssignableFrom(X) && !X.IsAbstract).ToList();
            if (type.Count > 0)
            {
                ITag tag = Activator.CreateInstance(type[0]) as ITag;
                tag.Parent = condtion;
                tag.Statement = statement;
                statement.Tags.Add(tag);
                if(condtion==null)
                {
                    statement.Child.Add(tag);
                }

                XmlElement element = (XmlElement)node;
                tag.Prepend = element.GetAttribute("Prepend");

                string str = element.GetAttribute("IsNotEmpty");
                if (!string.IsNullOrEmpty(str))
                {
                    Dynamic dynamic = new Dynamic
                    {
                        Parent = tag,
                        Sql = str,
                        Property = "IsNotEmpty",
                        Statement=statement
                    };
                    statement.Tags.Add(dynamic);
                }
              
                string Expression = element.GetAttribute("Dynamic");
                if (!string.IsNullOrEmpty(Expression))
                {
                    Dynamic tmp = new Dynamic
                    {
                        Parent = tag,
                        Sql = Expression,
                        Property = "Dynamic",
                        Statement = statement
                    };
                    statement.Tags.Add(tmp);
                }
                foreach (XmlNode child in node.ChildNodes)
                {
                    AnalysisNode(child, tag, statement);
                }
            }
           
        }
    }
}
