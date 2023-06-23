#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*项目名称 ：SmartXmlSql
*项目描述 ：
*命名空间 ：SmartXmlSql
*文件名称 ：ITag.cs
* 功能描述 ：ITag
* 创建时间 ：2020
*版本号   :   2020|V1.0.0.0 
---------------------------------------------------------------------
* Copyright @ jinyu 2020. All rights reserved.
---------------------------------------------------------------------

***********************************************************************/
#endregion


namespace SmartXmlSql.statements
{
    /// <summary>
    /// 表示一个sql语句节点
    /// </summary>
    public interface ITag
    {
        /// <summary>
        /// 解析的语句结果
        /// </summary>
        Statement Statement { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>

        ITag Parent { get; set; }

        /// <summary>
        /// SQL语句
        /// </summary>
        string Sql { get; set; }

        /// <summary>
        /// 预置词
        /// </summary>
        string Prepend { get; set; }


        /// <summary>
        /// 输出语句
        /// </summary>
        void BuildSql();

        /// <summary>
        /// 获取语句
        /// </summary>
        /// <returns></returns>
        string GetSql();
    }
}
