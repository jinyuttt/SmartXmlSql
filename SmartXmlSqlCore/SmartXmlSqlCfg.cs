#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*机器名称 ：DESKTOP-730PC6V
*项目名称 ：SmartXmlSql
*项目描述 ：
*命名空间 ：SmartXmlSql
*文件名称 ：SmartXmlSqlCfg.cs
*版本号   :   2020|V1.0.0.0 
---------------------------------------------------------------------
* Copyright @ jinyu 2020. All rights reserved.
---------------------------------------------------------------------

***********************************************************************/
#endregion


namespace SmartXmlSql
{

    /// <summary>
    /// 配置
    /// </summary>
    public class SmartXmlSqlCfg
    {
        private static string xmlDir = "";
        /// <summary>
        /// xml文件根目录
        /// </summary>
        public static string XmlDir
        {
            get { return xmlDir; }
            set { xmlDir = value; }
        }
      
    }
}
