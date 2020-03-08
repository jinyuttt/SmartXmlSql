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

namespace SmartXmlCfgSql
{
    public class SmartSqlBuilder : IDisposable
    {
        public void Dispose()
        {
           
        }
        public  void Build()
        {

        }

        public void Find(string xml,string name)
        {
            XmlDocument document = new XmlDocument();
            document.Load(xml);
            XmlElement element = (XmlElement)document.DocumentElement;
           var nodelst= element.GetElementsByTagName(name);
            if(nodelst.Count>0)
            {
                Analysis analysis = new Analysis();
                Statement statement = new Statement();
                analysis.AnalysisStatement(nodelst[0], statement);
            }

        }
    }
}
