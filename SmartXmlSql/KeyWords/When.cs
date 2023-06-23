#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*项目名称 ：SmartXmlSql
*项目描述 ：
*命名空间 ：SmartXmlSql
*文件名称 ：When.cs
* 功能描述 ：When
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
using SmartXmlSql.statements;

namespace SmartXmlSql
{
    public class When : Tag
    {
     
        //父类实现
        public override string GetSql()
        {
            var dy = this.ChildTags.Where(X => X.GetType() == typeof(Dynamic));
            Dynamic tmp= null;
            foreach(var p in dy)
            {
                tmp =(Dynamic) p;
            }
            if(tmp!=null)
            {
                if(tmp.Condtion())
                {
                   var sql=  this.ChildTags.Where(X => X.GetType() == typeof(SqlText));
                    foreach(var p in sql)
                    {
                        return p.Sql;
                    }
                }
            }
            return "";
        }
    }
}
