#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*项目名称 ：SmartXmlSql
*项目描述 ：
*命名空间 ：SmartXmlSql
*文件名称 ：Tag.cs
* 功能描述 ：Tag
* 创建时间 ：2020
*版本号   :   2020|V1.0.0.0 
---------------------------------------------------------------------
* Copyright @ jinyu 2020. All rights reserved.
---------------------------------------------------------------------

***********************************************************************/
#endregion



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartXmlSql
{
    public abstract class Tag : ITag
    {
        readonly StringBuilder builder = new StringBuilder();
        public virtual String Prepend { get; set; }
        public String Property { get; set; }
        /// <summary>
        ///  验证属性是否存在，如果不存在则抛出异常 : TagRequiredFailException
        /// </summary>
        public bool Required { get; set; }
        public IList<ITag> ChildTags { get; set; }
        public ITag Parent { get; set; }
        public Statement Statement { get; set; }
        public string Sql { get ; set ; }

        public  virtual void BuildSql()
        {
            builder.Clear();
            //查找子节点有没有值
            if (ChildTags == null)
            {
                var kk = this.Statement.Tags.Where(X => X.Parent == this);
                ChildTags = new List<ITag>();
                foreach (ITag tag in kk)
                {
                    ChildTags.Add(tag);
                }


            }
            else
            {
                foreach (var tag in this.ChildTags)
                {
                    tag.BuildSql();
                    builder.Append(tag.GetSql());
                }

            }
        }

        public virtual string GetSql()
        {
            return this.Prepend+ builder.ToString();
        }
    }
}
