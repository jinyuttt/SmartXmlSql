#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*项目名称 ：SmartXmlSql
*项目描述 ：
*命名空间 ：SmartXmlSql
*文件名称 ：SqlText.cs
* 功能描述 ：SqlText
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
    /// SQL语句非参数(update XXX,select XXX)
    /// </summary>
    public class SqlText : Tag
    {

        public override string GetSql()
        {
            string tmp = base.GetSql();
            return Sql + tmp;
        }



    }
}
