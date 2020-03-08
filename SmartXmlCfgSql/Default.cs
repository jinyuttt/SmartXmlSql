#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*项目名称 ：SmartXmlSql
*项目描述 ：
*命名空间 ：SmartXmlSql
*文件名称 ：Default.cs
* 功能描述 ：Default
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

namespace SmartXmlCfgSql
{
    /// <summary>
    /// 只是为了Switch或者末尾节点匹配
    /// 直接把字符串添加到后面
    /// </summary>
    public class Default:Tag
    {
        public override string GetSql()
        {
            string tmp= base.GetSql();
            if(!string.IsNullOrEmpty(this.Sql))
            {
                return tmp + Sql;
            }
            return tmp;
        }
    }
}
