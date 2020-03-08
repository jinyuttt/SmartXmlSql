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
namespace SmartXmlSql
{

    /// <summary>
    /// 解析XML
    /// </summary>
    public class Analysis
    {
        public void AnalysisStatement(XmlNode node, Statement statement)
        {

            //说明是第一级
            statement.Name = node.Name;
            //查找
            SqlText sql = new SqlText();
            sql.Sql = node.InnerText;
            statement.Tags.Add(sql);
            foreach (XmlNode child in node.ChildNodes)
            {
                AnalysisNode(child, sql, statement);
            }

        }
        void AnalysisNode(XmlNode node, ITag condtion, Statement statement)
        {

            //通过名称映射
            var type = Assembly.GetExecutingAssembly().DefinedTypes.Where(X => X.Name == node.Name && typeof(ITag).IsAssignableFrom(X) && !X.IsAbstract).ToList();
            if (type.Count > 0)
            {
                ITag tag = Activator.CreateInstance(type[0]) as ITag;
                tag.Parent = condtion;
                tag.Sql = node.InnerText;
                statement.Tags.Add(tag);

                XmlElement element = (XmlElement)node;
                tag.Prepend = element.GetAttribute("Prepend");

                string str = element.GetAttribute("IsNotEmpty");
                Dynamic dynamic = new Dynamic
                {
                    Parent = tag,
                    Sql = str,
                    Property = "IsNotEmpty"
                };
                statement.Tags.Add(dynamic);
                string Expression = element.GetAttribute("Dynamic");
                Dynamic tmp = new Dynamic
                {
                    Parent = tag,
                    Sql = Expression,
                    Property = "Dynamic"
                };
                statement.Tags.Add(tmp);
            }
            foreach (XmlNode child in node.ChildNodes)
            {
                AnalysisNode(child, condtion, statement);
            }
        }
    }
}
