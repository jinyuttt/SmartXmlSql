#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*项目名称 ：SmartXmlSql
*项目描述 ：
*命名空间 ：SmartXmlSql
*文件名称 ：choose.cs
* 功能描述 ：choose
* 创建时间 ：2020
*版本号   :   2020|V1.0.0.0 
---------------------------------------------------------------------
* Copyright @ jinyu 2020. All rights reserved.
---------------------------------------------------------------------

***********************************************************************/
#endregion

using System.Linq;
using SmartXmlSql.statements;

namespace SmartXmlSql
{
    public class Choose : Tag
    {
        //获取子节点的一个
        public override string GetSql()
        {
            string sql = "";
            var tags= this.ChildTags.Where(X => X.GetType() == typeof(When));
            foreach(ITag tag in tags)
            {
                sql += tag.GetSql();
            }
            if(string.IsNullOrEmpty(sql.Trim()))
            {
                 tags = this.ChildTags.Where(X => X.GetType() == typeof(Otherwise));
                foreach (ITag tag in tags)
                {
                    sql += tag.GetSql();
                }
            }
            return sql;
        }
    }
}
