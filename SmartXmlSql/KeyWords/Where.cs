#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*项目名称 ：SmartXmlSql
*项目描述 ：
*命名空间 ：SmartXmlSql
*文件名称 ：Where.cs
* 功能描述 ：Where
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
using System.Linq;

namespace SmartXmlSql
{
    public class Where : Tag
    {
       
        public  override  string GetSql()
        {
            string tmp = base.GetSql();
            if(tmp.StartsWith("And"))
            {
                tmp.Remove(0, 3);
            }
            if(tmp.StartsWith("Or"))
            {
                tmp.Remove(0, 2);
            }
            if (tmp.StartsWith(","))
            {
                tmp.Remove(0, 1);
            }
            if(!string.IsNullOrEmpty(tmp.Trim()))
            {
                tmp = " where " + tmp;
            }
            return tmp;
        }
    }
}
