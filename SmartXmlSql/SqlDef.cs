#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*项目名称 ：SmartXmlSql
*项目描述 ：
*命名空间 ：SmartXmlSql
*文件名称 ：SqlDef.cs
* 功能描述 ：SqlDef
* 创建时间 ：2020
*版本号   :   2020|V1.0.0.0 
---------------------------------------------------------------------
* Copyright @ jinyu 2020. All rights reserved.
---------------------------------------------------------------------

***********************************************************************/
#endregion


namespace SmartXmlSql
{
    /// <summary>
    /// 解析的SQL
    /// </summary>
    internal class SqlDef
    {

        /// <summary>
        /// SQL语句
        /// </summary>
        public string SQL { get; set; }

        /// <summary>
        /// Key关键字
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public string DB { get; set; }

        /// <summary>
        /// 访问类型
        /// </summary>
        public AcessType Acess { get; set; }
    }
}
